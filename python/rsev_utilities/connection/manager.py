from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Any

from ..database import ConnectionConfiguration
from .factory import ConnectionFactory
from .pool import ConnectionPool


class IConnectionManager(ABC):
    @abstractmethod
    def open(self, configuration: ConnectionConfiguration) -> Any: ...

    @abstractmethod
    def give_back(self, connection: Any) -> None: ...


class ConnectionManager(IConnectionManager):
    def __init__(self, factory: ConnectionFactory, configuration: ConnectionConfiguration):
        self._factory = factory
        self._pool = ConnectionPool(configuration.maximum_pool_size)

    def open(self, configuration: ConnectionConfiguration) -> Any:
        return self._pool.rent(lambda: self._factory.create(configuration))

    def give_back(self, connection: Any) -> None:
        self._pool.give_back(connection)

    def close(self) -> None:
        self._pool.clear()
