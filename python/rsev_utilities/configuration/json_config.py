# namespace: rsev_utilities.configuration.json_config

import json
from pathlib import Path


class JsonConfig:

    @staticmethod
    def load(path: str):
        return json.loads(Path(path).read_text(encoding="utf-8"))

    @staticmethod
    def save(path: str, data):
        Path(path).write_text(json.dumps(data, indent=2), encoding="utf-8")
