# namespace: rsev_utilities.logging.advanced_logger

import logging

class AdvancedLogger:

    @staticmethod
    def create(name: str, level=logging.INFO):
        logger = logging.getLogger(name)
        logger.setLevel(level)

        if not logger.handlers:
            ch = logging.StreamHandler()
            formatter = logging.Formatter(
                "[%(asctime)s] [%(levelname)s] [%(name)s] %(message)s"
            )
            ch.setFormatter(formatter)
            logger.addHandler(ch)

        return logger