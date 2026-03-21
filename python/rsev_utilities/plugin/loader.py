# namespace: rsev_utilities.plugin.loader

import importlib
import os
import sys

from .base_plugin import BasePlugin
from .plugin_context import PluginContext


class PluginLoader:

    def __init__(self):
        self.plugins = []
        self.context = PluginContext()

    def load_from_directory(self, directory: str):
        sys.path.insert(0, directory)

        for file in os.listdir(directory):
            if file.endswith(".py") and not file.startswith("_"):
                module_name = file[:-3]
                module = importlib.import_module(module_name)

                for attr in dir(module):
                    obj = getattr(module, attr)
                    if isinstance(obj, type) and issubclass(obj, BasePlugin) and obj != BasePlugin:
                        instance = obj()
                        instance.on_load()
                        self.plugins.append(instance)

    def enable_all(self):
        for plugin in self.plugins:
            plugin.on_enable()

    def disable_all(self):
        for plugin in self.plugins:
            plugin.on_disable()
