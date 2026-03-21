class IRuntimeContext:

    def set(self, key: str, value):
        raise NotImplementedError()

    def get(self, key: str, default=None):
        raise NotImplementedError()

    def try_get(self, key: str):
        raise NotImplementedError()
