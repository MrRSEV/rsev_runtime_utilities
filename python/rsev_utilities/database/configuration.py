from dataclasses import dataclass


@dataclass(frozen=True)
class ConnectionConfiguration:
    provider: str
    connection_string: str
    username: str = ""
    password: str = ""
    minimum_pool_size: int = 1
    maximum_pool_size: int = 10

    def __post_init__(self):
        if not self.provider.strip() or not self.connection_string.strip():
            raise ValueError("provider and connection_string are required")
        if self.minimum_pool_size < 0 or self.maximum_pool_size < 1 or self.minimum_pool_size > self.maximum_pool_size:
            raise ValueError("invalid pool size bounds")
