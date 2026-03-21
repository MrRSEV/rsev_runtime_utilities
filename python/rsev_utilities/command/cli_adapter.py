# namespace: rsev_utilities.command.cli_adapter

import sys

from .registry import CommandRegistry


class CLIAdapter:

    def __init__(self, registry: CommandRegistry):
        self.registry = registry

    def run(self):
        if len(sys.argv) < 2:
            raise ValueError("No command provided")
        name = sys.argv[1]
        args = sys.argv[2:]
        return self.registry.execute(name, *args)
