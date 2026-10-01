from ..configuration.conf_config import ConfConfig
from ..logging.base_logger import BaseLogger
from ..api import RuntimeApiHost
from .processing import RuntimeProcessController


class BaseRuntimeContext:

    def __init__(self):
        self.config = self.create_default_config()
        self.logger = self.create_logger()
        self.process_controller = self.create_process_controller()
        self.api_host = self.create_api_host()
        self.global_state = {}
        self.is_running = False

    def create_default_config(self):
        return ConfConfig()

    def create_logger(self):
        return BaseLogger.get(self.__class__.__name__)

    def create_process_controller(self):
        return RuntimeProcessController(self.logger)

    def create_api_host(self):
        return RuntimeApiHost(logger=self.logger)

    def set(self, key: str, value):
        self.global_state[key] = value

    def get(self, key: str, default=None):
        return self.global_state.get(key, default)

    def try_get(self, key: str):
        return key in self.global_state, self.global_state.get(key)
