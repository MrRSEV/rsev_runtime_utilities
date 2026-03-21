from dataclasses import dataclass, field


@dataclass
class ShutdownSummary:
    successful: bool = True
    exit_code: int = 0
    messages: list[str] = field(default_factory=list)
