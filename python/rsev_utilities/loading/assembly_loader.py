import importlib


class AssemblyLoader:

    @staticmethod
    def load(module_name: str):
        return importlib.import_module(module_name)
