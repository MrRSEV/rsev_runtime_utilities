from __future__ import annotations

from typing import Generic, TypeVar


TEvent = TypeVar("TEvent")


class EventHandler(Generic[TEvent]):
    """Synchronous/asynchronous event handler contract."""

    def handle(self, sender, event: TEvent) -> None:
        self.on_handle(sender, event)

    async def handle_async(self, sender, event: TEvent) -> None:
        result = self.on_handle_async(sender, event)
        if hasattr(result, "__await__"):
            await result

    def on_handle(self, sender, event: TEvent) -> None:
        pass

    async def on_handle_async(self, sender, event: TEvent) -> None:
        self.on_handle(sender, event)


class EventListener(Generic[TEvent]):
    """Synchronous/asynchronous event listener contract."""

    def listen(self, sender, event: TEvent) -> None:
        self.on_listen(sender, event)

    async def listen_async(self, sender, event: TEvent) -> None:
        result = self.on_listen_async(sender, event)
        if hasattr(result, "__await__"):
            await result

    def on_listen(self, sender, event: TEvent) -> None:
        pass

    async def on_listen_async(self, sender, event: TEvent) -> None:
        self.on_listen(sender, event)


BaseEventHandler = EventHandler
BaseEventListener = EventListener
