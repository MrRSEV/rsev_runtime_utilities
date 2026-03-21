# namespace: rsev_utilities.next_version.feature_flags

class FeatureFlags:

    WEB_SERVER = False
    WEBSOCKET = False
    SCHEDULER = False
    METRICS = False
    AI = False

    @staticmethod
    def all_flags():
        return {
            "web_server": FeatureFlags.WEB_SERVER,
            "websocket": FeatureFlags.WEBSOCKET,
            "scheduler": FeatureFlags.SCHEDULER,
            "metrics": FeatureFlags.METRICS,
            "ai": FeatureFlags.AI,
        }