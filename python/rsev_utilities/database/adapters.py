from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Any, Callable

from .configuration import ConnectionConfiguration


class IDatabaseAdapter(ABC):
    provider = ""

    @abstractmethod
    def create_connection(self, configuration: ConnectionConfiguration) -> Any:
        """Create a connection through an injected DB-API compatible factory."""


class _ProviderAdapter(IDatabaseAdapter):
    def __init__(self, connection_factory: Callable[[ConnectionConfiguration], Any]):
        self._connection_factory = connection_factory

    def create_connection(self, configuration: ConnectionConfiguration) -> Any:
        return self._connection_factory(configuration)


class MySqlAdapter(_ProviderAdapter):
    provider = "mysql"


class SqlServerAdapter(_ProviderAdapter):
    provider = "mssql"


class PostgreSqlAdapter(_ProviderAdapter):
    provider = "postgresql"


class SqliteAdapter(_ProviderAdapter):
    provider = "sqlite"
