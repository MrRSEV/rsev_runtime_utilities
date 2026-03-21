# namespace: rsev_utilities.command.base_command

from .command import Command


class BaseCommand(Command):

    def __init__(self, name: str = "", description: str = ""):
        self.name = name
        self.description = description
