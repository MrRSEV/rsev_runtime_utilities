from abc import ABC, abstractmethod


class IGeneratorPlugin(ABC):
    name = ""
    version = "1.0.0"

    @abstractmethod
    def generate(self, context) -> None:
        """Extend a framework generation/runtime context."""
