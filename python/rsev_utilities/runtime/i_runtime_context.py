class IRuntimeContext:

    @property
    def process_controller(self):
        raise NotImplementedError()

    @property
    def api_host(self):
        raise NotImplementedError()

    def set(self, key: str, value):
        raise NotImplementedError()

    def get(self, key: str, default=None):
        raise NotImplementedError()

    def try_get(self, key: str):
        raise NotImplementedError()
