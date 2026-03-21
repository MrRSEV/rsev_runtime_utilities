class ISmtpClient:

    def send(self, options):
        raise NotImplementedError()

    async def send_async(self, options):
        raise NotImplementedError()
