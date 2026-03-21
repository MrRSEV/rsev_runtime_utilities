# namespace: rsev_utilities.configuration.i_config


class IConfig:

    @staticmethod
    def load(path: str):
        raise NotImplementedError()

    @staticmethod
    def save(path: str, data):
        raise NotImplementedError()
