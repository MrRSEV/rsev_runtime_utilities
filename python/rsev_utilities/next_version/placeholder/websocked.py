# namespace: rsev_utilities.next_version.placeholders.websocket

from ..status import NextVersionStatus

class WebSocketServer:

    def __init__(self):
        NextVersionStatus.require()

    async def start(self):
        pass