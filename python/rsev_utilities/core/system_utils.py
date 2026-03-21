# namespace: rsev_utilities.core.system_utils

import os
import platform

class SystemUtils:

    @staticmethod
    def os_name() -> str:
        return platform.system()

    @staticmethod
    def env(key: str, default=None):
        return os.environ.get(key, default)

    @staticmethod
    def cwd() -> str:
        return os.getcwd()