class BaseController:

    def __init__(self, name: str = ""):
        self.name = name
        self.active = False

    def initialize(self, context=None):
        self.active = True
        self.on_initialize(context)

    async def initialize_async(self, context=None):
        self.active = True
        result = self.on_initialize_async(context)
        if result is not None:
            await result

    def shutdown(self, context=None):
        self.active = False
        self.on_shutdown(context)

    async def shutdown_async(self, context=None):
        self.active = False
        result = self.on_shutdown_async(context)
        if result is not None:
            await result

    def on_initialize(self, context=None):
        pass

    def on_initialize_async(self, context=None):
        return None

    def on_shutdown(self, context=None):
        pass

    def on_shutdown_async(self, context=None):
        return None
