# namespace: rsev_utilities.configuration.auto_reload_config

import os
import time


class AutoReloadConfig:

    def __init__(self, path: str, loader):
        self.path = path
        self.loader = loader
        self.last_modified = 0
        self.data = None

    def get(self):
        modified = os.path.getmtime(self.path)
        if self.data is None or modified > self.last_modified:
            self.data = self.loader(self.path)
            self.last_modified = modified
        return self.data

    def watch(self, interval: float = 1.0):
        while True:
            self.get()
            time.sleep(interval)
