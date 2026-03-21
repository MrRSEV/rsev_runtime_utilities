# namespace: rsev_utilities.next_version.placeholders.web_server

from ..status import NextVersionStatus

class WebServer:

    def __init__(self):
        NextVersionStatus.require()

    def start(self):
        pass