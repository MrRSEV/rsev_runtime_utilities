from __future__ import annotations

import asyncio
import inspect
import socketserver
import threading
from urllib.parse import urlsplit

from .endpoints import RuntimeEndpointController
from .models import EndpointRequest, EndpointResponse


class _RuntimeServer(socketserver.ThreadingTCPServer):
    allow_reuse_address = True
    daemon_threads = True


class _RequestHandler(socketserver.BaseRequestHandler):
    def handle(self):
        host: RuntimeApiHost = self.server.runtime_host
        try:
            data = b""
            content_length = 0
            while b"\r\n\r\n" not in data:
                chunk = self.request.recv(8192)
                if not chunk:
                    break
                data += chunk
                if len(data) > 1024 * 1024:
                    raise ValueError("request headers too large")

            header_bytes, _, body = data.partition(b"\r\n\r\n")
            lines = header_bytes.decode("iso-8859-1").split("\r\n")
            method, route, _ = lines[0].split(" ", 2)
            headers = {}
            for line in lines[1:]:
                if ":" in line:
                    key, value = line.split(":", 1)
                    headers[key.strip()] = value.strip()
            content_length = int(headers.get("Content-Length", "0"))
            while len(body) < content_length:
                body += self.request.recv(content_length - len(body))
            response = host.handle(EndpointRequest(method, urlsplit(route).path or "/", headers, body[:content_length]))
        except Exception as error:
            response = EndpointResponse.from_text(400, str(error))
            response.status_code = 500 if not isinstance(error, (ValueError, IndexError)) else 400
        self.request.sendall(response.to_http())


class RuntimeApiHost:
    """Small dependency-free TCP HTTP host for framework endpoints."""

    def __init__(self, host: str = "127.0.0.1", port: int = 0, *, logger=None):
        self.host = host
        self.port = port
        self.logger = logger
        self.endpoints = RuntimeEndpointController()
        self._server: _RuntimeServer | None = None
        self._thread: threading.Thread | None = None

    def register(self, endpoint) -> bool:
        return self.endpoints.register(endpoint)

    def unregister(self, route: str) -> bool:
        return self.endpoints.unregister(route)

    def resolve(self, route: str):
        return self.endpoints.resolve(route)

    def start(self) -> bool:
        if self._server is not None:
            return True
        self._server = _RuntimeServer((self.host, self.port), _RequestHandler)
        self._server.runtime_host = self
        self.host, self.port = self._server.server_address[:2]
        self._thread = threading.Thread(target=self._server.serve_forever, daemon=True)
        self._thread.start()
        return True

    def stop(self) -> None:
        if self._server is None:
            return
        self._server.shutdown()
        self._server.server_close()
        if self._thread is not None:
            self._thread.join(timeout=2)
        self._server = None
        self._thread = None

    def handle(self, request: EndpointRequest) -> EndpointResponse:
        endpoint = self.resolve(request.route)
        if endpoint is None:
            return EndpointResponse.from_text(404, "Not Found")
        try:
            result = endpoint.handle_async(request) if hasattr(endpoint, "handle_async") else endpoint.handle(request)
            if inspect.isawaitable(result):
                result = asyncio.run(result)
            if isinstance(result, EndpointResponse):
                return result
            if isinstance(result, (dict, list)):
                return EndpointResponse.from_json(result)
            return EndpointResponse.from_text(200, str(result))
        except Exception as error:
            return EndpointResponse.from_text(500, str(error))
