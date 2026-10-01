# namespace: rsev_utilities.events.event

class Event:
    """Base event with optional metadata used by the 2.0 event registry."""

    name = "Event"
    description = ""

    def __init__(self, name: str | None = None, description: str | None = None):
        if name is not None:
            self.name = name
        if description is not None:
            self.description = description

    @property
    def event_args_type(self):
        return type(self)
