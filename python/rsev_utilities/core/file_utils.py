# namespace: rsev_utilities.core.file_utils

from pathlib import Path
import gzip

class FileUtils:

    @staticmethod
    def read(path: str) -> str:
        p = Path(path)
        if p.suffix in (".gz", ".gzip"):
            with gzip.open(p, "rt", encoding="utf-8") as f:
                return f.read()
        return p.read_text(encoding="utf-8")

    @staticmethod
    def write(path: str, content: str):
        p = Path(path)
        if p.suffix in (".gz", ".gzip"):
            with gzip.open(p, "wt", encoding="utf-8") as f:
                f.write(content)
        else:
            p.write_text(content, encoding="utf-8")

    @staticmethod
    def exists(path: str) -> bool:
        return Path(path).exists()