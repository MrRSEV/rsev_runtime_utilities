from __future__ import annotations

import asyncio
import inspect
from threading import RLock
from typing import Callable, Generic, TypeVar


TEvent = TypeVar("TEvent")


class EventRegistry(Generic[TEvent]):
    """Thread-safe registry for handlers and listeners of one event type."""

    def __init__(self, event_name: str):
        if not event_name or not event_name.strip():
            raise ValueError("event_name must not be empty")
        self.event_name = event_name
        self._handlers: list[object] = []
        self._listeners: list[object] = []
        self._lock = RLock()

    @property
    def handlers(self) -> tuple[object, ...]:
        with self._lock:
            return tuple(self._handlers)

    @property
    def listeners(self) -> tuple[object, ...]:
        with self._lock:
            return tuple(self._listeners)

    def subscribe(self, component: object, *, listener: bool | None = None) -> None:
        """Register a handler/listener, or a callable as a listener."""
        if component is None:
            return
        if listener is None:
            listener = callable(component) and not hasattr(component, "handle")
        target = self._listeners if listener else self._handlers
        with self._lock:
            if component not in target:
                target.append(component)

    def subscribe_handler(self, handler: object) -> None:
        self.subscribe(handler, listener=False)

    def subscribe_listener(self, listener: object) -> None:
        self.subscribe(listener, listener=True)

    def unsubscribe(self, component: object, *, listener: bool | None = None) -> None:
        if listener is None:
            listener = component in self._listeners
        target = self._listeners if listener else self._handlers
        with self._lock:
            if component in target:
                target.remove(component)

    def publish(self, sender, event: TEvent) -> None:
        with self._lock:
            handlers = tuple(self._handlers)
            listeners = tuple(self._listeners)

        for component in (*handlers, *listeners):
            try:
                self._invoke_sync(component, sender, event)
            except Exception:
                # One faulty subscriber must not prevent the others from running.
                continue

    async def publish_async(self, sender, event: TEvent) -> None:
        with self._lock:
            components = (*self._handlers, *self._listeners)

        async def invoke(component):
            try:
                await self._invoke_async(component, sender, event)
            except Exception:
                pass

        await asyncio.gather(*(invoke(component) for component in components))

    @staticmethod
    def _invoke_sync(component: object, sender, event) -> None:
        if hasattr(component, "handle"):
            result = component.handle(sender, event)
        elif hasattr(component, "listen"):
            result = component.listen(sender, event)
        else:
            result = component(event)
        if inspect.isawaitable(result):
            asyncio.run(result)

    @staticmethod
    async def _invoke_async(component: object, sender, event) -> None:
        method = None
        if hasattr(component, "handle_async"):
            method = component.handle_async
        elif hasattr(component, "listen_async"):
            method = component.listen_async
        elif hasattr(component, "handle"):
            method = component.handle
        elif hasattr(component, "listen"):
            method = component.listen
        else:
            method = component
        result = method(sender, event)
        if inspect.isawaitable(result):
            await result
