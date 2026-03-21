# namespace: rsev_utilities.di.container

class Container:

    def __init__(self):
        self._services = {}

    def register(self, key: str, instance):
        self._services[key] = instance

    def resolve(self, key: str):
        return self._services.get(key)