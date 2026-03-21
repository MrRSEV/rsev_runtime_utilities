# namespace: rsev_utilities.plugin.entry_point_loader

import importlib.metadata

from .base_plugin import BasePlugin


class EntryPointPluginLoader:

    def load(self, group: str = "rsev.plugins"):
        plugins = []

        for entry_point in importlib.metadata.entry_points().select(group=group):
            plugin_class = entry_point.load()
            if issubclass(plugin_class, BasePlugin):
                instance = plugin_class()
                instance.on_load()
                plugins.append(instance)

        return plugins
