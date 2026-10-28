import secrets


class KeyManager:
    def generate(self, size: int = 32) -> bytes:
        if size < 16:
            raise ValueError("size must be at least 16")
        return secrets.token_bytes(size)
