# namespace: rsev_utilities.configuration.yaml_config

from pathlib import Path

try:
    import yaml
except ModuleNotFoundError:
    yaml = None


class YamlConfig:

    @staticmethod
    def load(path: str):
        if yaml is None:
            raise ModuleNotFoundError("PyYAML is required to load YAML configuration files.")
        return yaml.safe_load(Path(path).read_text(encoding="utf-8"))

    @staticmethod
    def save(path: str, data):
        if yaml is None:
            raise ModuleNotFoundError("PyYAML is required to save YAML configuration files.")
        Path(path).write_text(yaml.safe_dump(data), encoding="utf-8")
