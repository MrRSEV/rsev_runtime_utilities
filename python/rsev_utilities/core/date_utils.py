# namespace: rsev_utilities.core.date_utils

from datetime import datetime

class DateUtils:

    @staticmethod
    def now() -> datetime:
        return datetime.now()

    @staticmethod
    def format(dt: datetime, fmt: str = "%Y-%m-%d %H:%M:%S") -> str:
        return dt.strftime(fmt)

    @staticmethod
    def parse(value: str, fmt: str = "%Y-%m-%d %H:%M:%S") -> datetime:
        return datetime.strptime(value, fmt)