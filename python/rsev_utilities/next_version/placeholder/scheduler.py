# namespace: rsev_utilities.next_version.placeholders.scheduler

from ..status import NextVersionStatus

class Scheduler:

    def __init__(self):
        NextVersionStatus.require()

    def schedule(self, func, interval: int):
        pass