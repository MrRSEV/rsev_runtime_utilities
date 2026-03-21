# namespace: rsev_utilities.next_version.status

class NextVersionStatus:

    ENABLED = False
    VERSION = "0.0.0-dev"

    @staticmethod
    def is_enabled() -> bool:
        return NextVersionStatus.ENABLED

    @staticmethod
    def require():
        if not NextVersionStatus.ENABLED:
            raise RuntimeError(
                "Next Version is not available in this version. "
                "This is a reserved feature set for future releases."
            )