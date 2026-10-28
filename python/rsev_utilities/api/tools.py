from __future__ import annotations

import time
import urllib.error
import urllib.request
from collections import deque
from dataclasses import dataclass, field
from datetime import datetime, timedelta, timezone


@dataclass(frozen=True)
class ApiRequest:
    method: str
    url: str
    headers: dict[str, str] = field(default_factory=dict)
    body: bytes = b""


@dataclass(frozen=True)
class ApiResponse:
    status_code: int
    headers: dict[str, str]
    body: bytes

    @property
    def is_success(self) -> bool:
        return 200 <= self.status_code < 300


@dataclass(frozen=True)
class RetryPolicy:
    max_attempts: int = 3
    delay_seconds: float = 0.2


class RateLimitPolicy:
    def __init__(self, requests_per_window: int = 60, window_seconds: float = 60.0):
        if requests_per_window < 1 or window_seconds <= 0:
            raise ValueError("invalid rate limit")
        self.requests_per_window = requests_per_window
        self.window_seconds = window_seconds
        self._requests: deque[float] = deque()

    def try_acquire(self) -> bool:
        now = time.monotonic()
        while self._requests and self._requests[0] <= now - self.window_seconds:
            self._requests.popleft()
        if len(self._requests) >= self.requests_per_window:
            return False
        self._requests.append(now)
        return True


class IApiClient:
    def send(self, request: ApiRequest) -> ApiResponse:
        raise NotImplementedError


class HttpApiClient(IApiClient):
    def __init__(self, retry_policy: RetryPolicy | None = None, rate_limit_policy: RateLimitPolicy | None = None):
        self.retry_policy = retry_policy or RetryPolicy()
        self.rate_limit_policy = rate_limit_policy or RateLimitPolicy()

    def send(self, request: ApiRequest) -> ApiResponse:
        for attempt in range(max(1, self.retry_policy.max_attempts)):
            if not self.rate_limit_policy.try_acquire():
                raise RuntimeError("API rate limit reached")
            try:
                response = urllib.request.urlopen(urllib.request.Request(request.url, request.body, request.headers, method=request.method), timeout=30)
                result = ApiResponse(response.status, dict(response.headers.items()), response.read())
            except urllib.error.HTTPError as error:
                result = ApiResponse(error.code, dict(error.headers.items()), error.read())
            except urllib.error.URLError:
                if attempt + 1 >= max(1, self.retry_policy.max_attempts):
                    raise
                time.sleep(self.retry_policy.delay_seconds)
                continue
            if result.status_code < 500 or attempt + 1 >= max(1, self.retry_policy.max_attempts):
                return result
            time.sleep(self.retry_policy.delay_seconds)
        raise RuntimeError("request retry loop ended unexpectedly")
