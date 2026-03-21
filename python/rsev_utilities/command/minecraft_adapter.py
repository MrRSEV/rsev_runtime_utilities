# namespace: rsev_utilities.command.minecraft_adapter


class MinecraftAdapter:

    def __init__(self, registry):
        self.registry = registry

    def execute(self, name: str, *args):
        return self.registry.execute(name, *args)
