from .i_smtp_client import ISmtpClient


class BaseSmtpClient(ISmtpClient):

    def send(self, options):
        raise NotImplementedError()

    async def send_async(self, options):
        raise NotImplementedError()
