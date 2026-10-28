from __future__ import annotations

import hashlib
import hmac
from abc import ABC, abstractmethod


class ICryptoProvider(ABC):
    algorithm = ""

    @abstractmethod
    def compute(self, data: bytes) -> bytes: ...

    def verify(self, data: bytes, signature: bytes) -> bool:
        return hmac.compare_digest(self.compute(data), signature)


class HamacProvider(ICryptoProvider):
    algorithm = "HAMAC-SHA256"

    def __init__(self, key: bytes):
        if not key:
            raise ValueError("key is required")
        self._key = bytes(key)

    def compute(self, data: bytes) -> bytes:
        return hmac.new(self._key, data, hashlib.sha256).digest()


class Sha256Provider(ICryptoProvider):
    algorithm = "SHA-256"

    def compute(self, data: bytes) -> bytes:
        return hashlib.sha256(data).digest()
