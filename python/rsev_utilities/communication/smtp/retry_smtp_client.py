# namespace: rsev_utilities.communication.smtp.retry_smtp_client

import time

from .mail_options import MailOptions
from .raw_smtp_client import RawSmtpClient


class RetrySmtpClient:

    @staticmethod
    def send(options: MailOptions, retries=3, delay=2):
        for attempt in range(retries):
            try:
                RawSmtpClient.send(options)
                return
            except Exception:
                if attempt == retries - 1:
                    raise
                time.sleep(delay)
