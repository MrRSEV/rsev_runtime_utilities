from typing import Any

from ..database import ConnectionConfiguration, IDatabaseAdapter


class ConnectionFactory:
    def __init__(self, adapter: IDatabaseAdapter):
        self.adapter = adapter

    def create(self, configuration: ConnectionConfiguration) -> Any:
        return self.adapter.create_connection(configuration)
