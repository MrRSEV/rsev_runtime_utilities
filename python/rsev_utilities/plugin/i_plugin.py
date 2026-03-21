class IPlugin:

    def on_load(self):
        raise NotImplementedError()

    def on_enable(self, context=None):
        raise NotImplementedError()

    async def on_enable_async(self, context=None):
        raise NotImplementedError()

    def on_disable(self, context=None):
        raise NotImplementedError()

    async def on_disable_async(self, context=None):
        raise NotImplementedError()

    def on_reload(self, context=None):
        raise NotImplementedError()

    async def on_reload_async(self, context=None):
        raise NotImplementedError()
