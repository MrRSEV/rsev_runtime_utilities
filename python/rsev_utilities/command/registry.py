# namespace: rsev_utilities.command.registry


class CommandRegistry:

    def __init__(self):
        self.commands = {}

    def register(self, command):
        self.commands[command.name] = command

    def execute(self, name: str, *args):
        if name not in self.commands:
            raise ValueError("Command not found")
        return self.commands[name].execute(*args)
