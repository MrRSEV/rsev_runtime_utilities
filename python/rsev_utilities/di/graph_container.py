# namespace: rsev_utilities.di.graph_container

import inspect

class GraphContainer:

    def __init__(self):
        self._providers = {}

    def register(self, cls):
        self._providers[cls] = cls

    def resolve(self, cls):
        constructor = inspect.signature(cls.__init__)
        params = constructor.parameters

        kwargs = {}
        for name, param in params.items():
            if name == "self":
                continue
            dep = param.annotation
            if dep in self._providers:
                kwargs[name] = self.resolve(dep)

        return cls(**kwargs)