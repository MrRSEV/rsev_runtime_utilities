# namespace: rsev_utilities.communication.smtp.system_mail

from .async_smtp_client import AsyncSmtpClient
from .mail_options import MailOptions
from .raw_smtp_client import RawSmtpClient


class SystemMail:

    def __init__(self, options: MailOptions | None = None, smtp_client=None):
        self.options = options
        self.smtp_client = smtp_client or RawSmtpClient()

    def set_options(self, options: MailOptions):
        self.options = options

    def set_smtp_client(self, smtp_client):
        self.smtp_client = smtp_client

    def send(self):
        if self.options is None:
            return
        self.smtp_client.send(self.options)

    def send_async(self):
        if self.options is None:
            return None
        return AsyncSmtpClient.send(self.options)

    @staticmethod
    def send_simple(host, port, user, password, to, subject, body):
        options = MailOptions(
            host=host,
            port=port,
            username=user,
            password=password,
            from_addr=user,
            to_addrs=[to],
            subject=subject,
            body=body,
        )
        RawSmtpClient.send(options)

    @staticmethod
    async def send_simple_async(host, port, user, password, to, subject, body):
        options = MailOptions(
            host=host,
            port=port,
            username=user,
            password=password,
            from_addr=user,
            to_addrs=[to],
            subject=subject,
            body=body,
        )
        await AsyncSmtpClient.send(options)
