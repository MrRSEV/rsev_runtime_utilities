from __future__ import annotations

import importlib.util
import inspect
from dataclasses import dataclass
from pathlib import Path

from .i_generator_plugin import IGeneratorPlugin


@dataclass(frozen=True)
class PluginDescriptor:
    name: str
    version: str
    plugin: IGeneratorPlugin


class GeneratorPluginRegistry:
    def __init__(self):
        self._plugins: dict[str, PluginDescriptor] = {}

    def register(self, plugin: IGeneratorPlugin) -> None:
        if not plugin.name:
            raise ValueError("plugin name is required")
        if plugin.name in self._plugins:
            raise ValueError(f"plugin '{plugin.name}' is already registered")
        self._plugins[plugin.name] = PluginDescriptor(plugin.name, plugin.version, plugin)

    def discover(self, directory: str | Path) -> None:
        for path in sorted(Path(directory).glob("*.py")):
            if path.name.startswith("_"):
                continue
            spec = importlib.util.spec_from_file_location(f"rsev_plugin_{path.stem}", path)
            if spec is None or spec.loader is None:
                continue
            module = importlib.util.module_from_spec(spec)
            spec.loader.exec_module(module)
            for _, candidate in inspect.getmembers(module, inspect.isclass):
                if candidate is not IGeneratorPlugin and issubclass(candidate, IGeneratorPlugin):
                    self.register(candidate())

    def generate(self, context) -> None:
        for descriptor in sorted(self._plugins.values(), key=lambda value: value.name):
            descriptor.plugin.generate(context)
