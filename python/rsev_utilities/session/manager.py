from __future__ import annotations

import secrets
from datetime import timedelta, datetime, timezone
from abc import ABC, abstractmethod

from .models import RefreshToken, SessionContext
from .provider import IAccessTokenProvider


class ISessionManager(ABC):
    @abstractmethod
    def create(self, subject: str) -> SessionContext: ...

    @abstractmethod
    def is_valid(self, context: SessionContext) -> bool: ...


class SessionManager(ISessionManager):
    def __init__(self, token_provider: IAccessTokenProvider, access_lifetime: timedelta = timedelta(hours=1), refresh_lifetime: timedelta = timedelta(days=30)):
        self.token_provider = token_provider
        self.access_lifetime = access_lifetime
        self.refresh_lifetime = refresh_lifetime

    def create(self, subject: str) -> SessionContext:
        return SessionContext(subject, self.token_provider.create(subject, self.access_lifetime), RefreshToken(secrets.token_urlsafe(32), datetime.now(timezone.utc) + self.refresh_lifetime))

    def is_valid(self, context: SessionContext) -> bool:
        return self.token_provider.validate(context.access_token) and not context.refresh_token.is_expired
