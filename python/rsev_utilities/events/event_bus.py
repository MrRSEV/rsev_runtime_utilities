# namespace: rsev_utilities.events.event_bus

from __future__ import annotations

import inspect
from threading import RLock
from typing import Callable, Dict, List, Type

from .event import Event
from .event_registry import EventRegistry


class EventBus:
    """Central event bus with the original subscribe/publish API and registries."""

    def __init__(self):
        self.listeners: Dict[Type[Event], List[Callable]] = {}
        self._registries: dict[tuple[str, type], EventRegistry] = {}
        self._lock = RLock()

    def subscribe(self, event_type: Type[Event], listener: Callable):
        # Preserve the 1.x convenience API.
        self.listeners.setdefault(event_type, []).append(listener)

    def publish(self, event: Event):
        for event_type, listeners in tuple(self.listeners.items()):
            if isinstance(event, event_type):
                for listener in tuple(listeners):
                    listener(event)

        event_name = getattr(event, "name", type(event).__name__)
        registry = self.get_registry(event_name, type(event))
        if registry is not None:
            registry.publish(self, event)

    def register_registry(self, registry: EventRegistry, event_type: type | None = None) -> None:
        if registry is None:
            raise ValueError("registry must not be None")
        event_type = event_type or Event
        with self._lock:
            self._registries[(registry.event_name, event_type)] = registry

    def get_registry(self, event_name: str, event_type: type = Event) -> EventRegistry | None:
        with self._lock:
            return self._registries.get((event_name, event_type)) or self._registries.get(
                (event_name, Event)
            )

    def has_registry(self, event_name: str, event_type: type = Event) -> bool:
        return self.get_registry(event_name, event_type) is not None

    def get_all_event_names(self) -> list[str]:
        with self._lock:
            return list(dict.fromkeys(name for name, _ in self._registries))


BaseEventBus = EventBus
