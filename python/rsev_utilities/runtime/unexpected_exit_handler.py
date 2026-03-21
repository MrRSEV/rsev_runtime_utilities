import traceback

from .shutdown_summary import ShutdownSummary


class UnexpectedExitHandler:

    @staticmethod
    def handle(error: BaseException) -> ShutdownSummary:
        return ShutdownSummary(
            successful=False,
            exit_code=1,
            messages=[str(error), traceback.format_exc()],
        )
