# namespace: rsev_utilities.configuration.xml_config

import xml.etree.ElementTree as ET
from pathlib import Path


class XmlConfig:

    @staticmethod
    def load(path: str):
        return ET.parse(path).getroot()

    @staticmethod
    def save(path: str, root):
        tree = ET.ElementTree(root)
        tree.write(Path(path), encoding="utf-8", xml_declaration=True)
