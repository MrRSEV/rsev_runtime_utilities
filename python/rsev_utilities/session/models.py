from dataclasses import dataclass
from datetime import datetime


@dataclass(frozen=True)
class AccessToken:
    value: str
    expires_at: datetime

    @property
    def is_expired(self) -> bool:
        from datetime import datetime, timezone
        return datetime.now(timezone.utc) >= self.expires_at


@dataclass(frozen=True)
class RefreshToken:
    value: str
    expires_at: datetime

    @property
    def is_expired(self) -> bool:
        from datetime import datetime, timezone
        return datetime.now(timezone.utc) >= self.expires_at


@dataclass(frozen=True)
class SessionContext:
    subject: str
    access_token: AccessToken
    refresh_token: RefreshToken
