# namespace: rsev_utilities.communication.smtp.mail_options

from dataclasses import dataclass
from typing import List


@dataclass
class MailOptions:
    host: str
    port: int
    username: str
    password: str
    from_addr: str
    to_addrs: List[str]
    subject: str
    body: str
