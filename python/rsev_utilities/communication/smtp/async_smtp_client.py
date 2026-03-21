# namespace: rsev_utilities.communication.smtp.async_smtp_client

import asyncio

from .mail_options import MailOptions
from .raw_smtp_client import RawSmtpClient


class AsyncSmtpClient:

    @staticmethod
    async def send(options: MailOptions):
        loop = asyncio.get_event_loop()
        await loop.run_in_executor(None, RawSmtpClient.send, options)
