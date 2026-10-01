from __future__ import annotations

import http.client
import json
import ssl
from dataclasses import dataclass, field
from datetime import datetime, timezone
from urllib.parse import urlsplit


@dataclass
class RuntimeRequest:
    route: str
    payload: object = None
    headers: dict[str, str] = field(default_factory=dict)
    session_id: str | None = None


class RuntimeRequestBuilder:
    def __init__(self):
        self._request = RuntimeRequest("/")

    def with_route(self, route: str):
        self._request.route = route
        return self

    def with_payload(self, payload):
        self._request.payload = payload
        return self

    def with_header(self, name: str, value: str):
        self._request.headers[name] = value
        return self

    def bind_session(self, session: "RuntimeSession"):
        self._request.session_id = session.session_id
        return self

    def build(self) -> RuntimeRequest:
        return RuntimeRequest(
            self._request.route,
            self._request.payload,
            dict(self._request.headers),
            self._request.session_id,
        )

    def build_bytes(self) -> bytes:
        request = self.build()
        body = b"" if request.payload is None else json.dumps(request.payload).encode("utf-8")
        headers = {"Content-Length": str(len(body)), "Content-Type": "application/json", **request.headers}
        if request.session_id:
            headers["X-Session-Id"] = request.session_id
        lines = [f"POST {request.route} HTTP/1.1"]
        lines.extend(f"{key}: {value}" for key, value in headers.items())
        return ("\r\n".join(lines) + "\r\n\r\n").encode("ascii") + body


@dataclass
class RuntimeResponse:
    status_code: int
    body: bytes = b""
    headers: dict[str, str] = field(default_factory=dict)
    error_message: str | None = None

    @property
    def is_error(self) -> bool:
        return self.status_code >= 400

    def get_body_as_text(self) -> str:
        return self.body.decode("utf-8", errors="replace")


class RuntimeSession:
    def __init__(self, session_id: str, expires_at: datetime | None = None, metadata: dict | None = None):
        self.session_id = session_id
        self.expires_at = expires_at
        self.metadata = metadata or {}

    @property
    def is_expired(self) -> bool:
        return self.expires_at is not None and self.expires_at <= datetime.now(timezone.utc)

    async def renew_async(self) -> None:
        """Override in applications that can renew sessions."""


class RuntimeApiClient:
    def __init__(self, host: str, port: int, *, use_tls: bool = False, ssl_context=None):
        self.host = host
        self.port = port
        self.use_tls = use_tls
        self.ssl_context = ssl_context
        self.session: RuntimeSession | None = None

    def set_session(self, session: RuntimeSession | None):
        self.session = session
        return self

    def call(self, route: str, payload=None) -> RuntimeResponse:
        return self.send(RuntimeRequestBuilder().with_route(route).with_payload(payload).build())

    def send(self, request: RuntimeRequest) -> RuntimeResponse:
        connection_type = http.client.HTTPSConnection if self.use_tls else http.client.HTTPConnection
        options = {"context": self.ssl_context} if self.use_tls and self.ssl_context else {}
        connection = connection_type(self.host, self.port, timeout=30, **options)
        try:
            headers = dict(request.headers)
            body = None if request.payload is None else json.dumps(request.payload).encode("utf-8")
            if body is not None:
                headers.setdefault("Content-Type", "application/json")
            if self.session:
                if self.session.is_expired:
                    raise RuntimeError("session renewal is required before a synchronous call")
                headers["X-Session-Id"] = self.session.session_id
            connection.request("POST", request.route, body=body, headers=headers)
            response = connection.getresponse()
            result = RuntimeResponse(response.status, response.read(), dict(response.getheaders()))
            if result.is_error:
                result.error_message = result.get_body_as_text()
            return result
        finally:
            connection.close()
