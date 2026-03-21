from ..configuration.conf_config import ConfConfig
from ..logging.base_logger import BaseLogger


class BaseRuntimeContext:

    def __init__(self):
        self.config = self.create_default_config()
        self.logger = self.create_logger()
        self.global_state = {}
        self.is_running = False

    def create_default_config(self):
        return ConfConfig()

    def create_logger(self):
        return BaseLogger.get(self.__class__.__name__)

    def set(self, key: str, value):
        self.global_state[key] = value

    def get(self, key: str, default=None):
        return self.global_state.get(key, default)

    def try_get(self, key: str):
        return key in self.global_state, self.global_state.get(key)
