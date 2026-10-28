from __future__ import annotations

import base64
import hashlib
import hmac
import json
import time
from abc import ABC, abstractmethod
from datetime import datetime, timedelta, timezone

from .models import AccessToken


def _encode(value: bytes) -> str:
    return base64.urlsafe_b64encode(value).rstrip(b"=").decode("ascii")


def _decode(value: str) -> bytes:
    return base64.urlsafe_b64decode(value + "=" * (-len(value) % 4))


class IAccessTokenProvider(ABC):
    @abstractmethod
    def create(self, subject: str, lifetime: timedelta) -> AccessToken: ...

    @abstractmethod
    def validate(self, token: AccessToken) -> bool: ...


class JwtTokenProvider(IAccessTokenProvider):
    def __init__(self, secret: bytes):
        if not secret:
            raise ValueError("secret is required")
        self._secret = bytes(secret)

    def create(self, subject: str, lifetime: timedelta) -> AccessToken:
        if not subject:
            raise ValueError("subject is required")
        expires = datetime.now(timezone.utc) + lifetime
        header = _encode(json.dumps({"alg": "HS256", "typ": "JWT"}, separators=(",", ":")).encode())
        payload = _encode(json.dumps({"sub": subject, "exp": int(expires.timestamp())}, separators=(",", ":")).encode())
        unsigned = f"{header}.{payload}".encode()
        signature = _encode(hmac.new(self._secret, unsigned, hashlib.sha256).digest())
        return AccessToken(f"{header}.{payload}.{signature}", expires)

    def validate(self, token: AccessToken) -> bool:
        if token.is_expired:
            return False
        try:
            header, payload, signature = token.value.split(".")
            expected = hmac.new(self._secret, f"{header}.{payload}".encode(), hashlib.sha256).digest()
            return hmac.compare_digest(expected, _decode(signature))
        except (ValueError, base64.binascii.Error):
            return False
