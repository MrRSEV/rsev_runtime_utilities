# namespace: rsev_utilities.next_version.placeholders.ai

from ..status import NextVersionStatus

class Metrics:

    def __init__(self):
        NextVersionStatus.require()

    def register(self, name: str):
        pass