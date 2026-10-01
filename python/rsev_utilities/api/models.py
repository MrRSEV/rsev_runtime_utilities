from __future__ import annotations

import json
from dataclasses import dataclass, field


@dataclass
class EndpointRequest:
    method: str
    route: str
    headers: dict[str, str] = field(default_factory=dict)
    body: bytes = b""

    @property
    def payload(self):
        if not self.body:
            return None
        try:
            return json.loads(self.body.decode("utf-8"))
        except (ValueError, UnicodeDecodeError):
            return self.body.decode("utf-8", errors="replace")


@dataclass
class EndpointResponse:
    status_code: int = 200
    body: bytes = b""
    headers: dict[str, str] = field(default_factory=dict)

    @classmethod
    def from_text(cls, status_code: int, text: str, *, content_type: str = "text/plain; charset=utf-8"):
        return cls(status_code, text.encode("utf-8"), {"Content-Type": content_type})

    @classmethod
    def from_json(cls, value, status_code: int = 200):
        return cls(
            status_code,
            json.dumps(value).encode("utf-8"),
            {"Content-Type": "application/json; charset=utf-8"},
        )

    def to_http(self) -> bytes:
        headers = {"Content-Length": str(len(self.body)), "Connection": "close", **self.headers}
        reason = {
            200: "OK",
            201: "Created",
            204: "No Content",
            400: "Bad Request",
            404: "Not Found",
            500: "Internal Server Error",
        }.get(self.status_code, "")
        lines = [f"HTTP/1.1 {self.status_code} {reason}".rstrip()]
        lines.extend(f"{key}: {value}" for key, value in headers.items())
        return ("\r\n".join(lines) + "\r\n\r\n").encode("ascii") + self.body
