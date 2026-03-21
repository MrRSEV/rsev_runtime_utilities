# namespace: rsev_utilities.command.discord_adapter


class DiscordAdapter:

    def __init__(self, registry):
        self.registry = registry

    def execute(self, name: str, *args):
        return self.registry.execute(name, *args)
