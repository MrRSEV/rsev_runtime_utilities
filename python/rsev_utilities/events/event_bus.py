# namespace: rsev_utilities.events.event_bus

from typing import Callable, Dict, List, Type
from .event import Event

class EventBus:

    def __init__(self):
        self.listeners: Dict[Type[Event], List[Callable]] = {}

    def subscribe(self, event_type: Type[Event], listener: Callable):
        self.listeners.setdefault(event_type, []).append(listener)

    def publish(self, event: Event):
        for event_type, listeners in self.listeners.items():
            if isinstance(event, event_type):
                for listener in listeners:
                    listener(event)