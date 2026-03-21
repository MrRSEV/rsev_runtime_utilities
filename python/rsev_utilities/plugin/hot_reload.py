# namespace: rsev_utilities.plugin.hot_reload

import importlib
import os
import sys
import time


class HotReloadManager:

    def __init__(self):
        self._timestamps = {}

    def watch(self, directory: str, callback, interval: float = 1.0):
        while True:
            for file in os.listdir(directory):
                if not file.endswith(".py") or file.startswith("_"):
                    continue

                path = os.path.join(directory, file)
                timestamp = os.path.getmtime(path)
                previous = self._timestamps.get(path)

                if previous is None:
                    self._timestamps[path] = timestamp
                    continue

                if timestamp > previous:
                    module_name = file[:-3]
                    if module_name in sys.modules:
                        importlib.reload(sys.modules[module_name])
                    callback(module_name)
                    self._timestamps[path] = timestamp

            time.sleep(interval)
