class IController:

    def initialize(self, context=None):
        raise NotImplementedError()

    async def initialize_async(self, context=None):
        raise NotImplementedError()

    def shutdown(self, context=None):
        raise NotImplementedError()

    async def shutdown_async(self, context=None):
        raise NotImplementedError()
