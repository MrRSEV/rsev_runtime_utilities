# namespace: rsev_utilities.core.string_utils

import re
from typing import List

class StringUtils:

    @staticmethod
    def is_empty(value: str) -> bool:
        return value is None or value.strip() == ""

    @staticmethod
    def join(values: List[str], sep: str = ",") -> str:
        return sep.join(values)

    @staticmethod
    def split(value: str, sep: str = ",") -> List[str]:
        return value.split(sep)

    @staticmethod
    def regex_match(pattern: str, value: str) -> bool:
        return re.match(pattern, value) is not None