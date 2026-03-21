# namespace: rsev_utilities.command.command_handler

from .registry import CommandRegistry


class CommandHandler(CommandRegistry):

    def register_command(self, command):
        self.register(command)
