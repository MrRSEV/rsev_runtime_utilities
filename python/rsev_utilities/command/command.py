# namespace: rsev_utilities.command.command


class Command:

    name = ""
    description = ""

    def execute(self, *args):
        raise NotImplementedError()
