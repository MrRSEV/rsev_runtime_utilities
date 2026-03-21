# namespace: rsev_utilities.configuration.conf_config

from configparser import ConfigParser


class ConfConfig:

    @staticmethod
    def load(path: str) -> ConfigParser:
        parser = ConfigParser()
        parser.read(path)
        return parser

    @staticmethod
    def save(path: str, parser: ConfigParser):
        with open(path, "w", encoding="utf-8") as file:
            parser.write(file)
