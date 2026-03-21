class ICommand:

    @property
    def name(self) -> str:
        raise NotImplementedError()

    @property
    def description(self) -> str:
        raise NotImplementedError()

    def execute(self, *args):
        raise NotImplementedError()
