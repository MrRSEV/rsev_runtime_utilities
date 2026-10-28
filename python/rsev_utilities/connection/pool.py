from __future__ import annotations

from queue import Empty, LifoQueue
from typing import Any, Callable


class ConnectionPool:
    def __init__(self, maximum_size: int = 10):
        if maximum_size < 1:
            raise ValueError("maximum_size must be positive")
        self._connections: LifoQueue[Any] = LifoQueue(maxsize=maximum_size)

    def rent(self, factory: Callable[[], Any]) -> Any:
        try:
            return self._connections.get_nowait()
        except Empty:
            return factory()

    def give_back(self, connection: Any) -> bool:
        try:
            self._connections.put_nowait(connection)
            return True
        except Exception:
            close = getattr(connection, "close", None)
            if close:
                close()
            return False

    def clear(self) -> None:
        while True:
            try:
                connection = self._connections.get_nowait()
            except Empty:
                return
            close = getattr(connection, "close", None)
            if close:
                close()
