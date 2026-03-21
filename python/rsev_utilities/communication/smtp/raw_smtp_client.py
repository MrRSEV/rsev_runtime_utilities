# namespace: rsev_utilities.communication.smtp.raw_smtp_client

import smtplib
from email.mime.text import MIMEText

from .mail_options import MailOptions


class RawSmtpClient:

    @staticmethod
    def send(options: MailOptions):
        msg = MIMEText(options.body)
        msg["Subject"] = options.subject
        msg["From"] = options.from_addr
        msg["To"] = ", ".join(options.to_addrs)

        with smtplib.SMTP(options.host, options.port) as server:
            server.starttls()
            server.login(options.username, options.password)
            server.sendmail(options.from_addr, options.to_addrs, msg.as_string())
