# namespace: rsev_utilities.logging.base_logger

import logging

class BaseLogger:

    @staticmethod
    def get(name: str) -> logging.Logger:
        logging.basicConfig(level=logging.INFO)
        return logging.getLogger(name)