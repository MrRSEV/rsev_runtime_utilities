from __future__ import annotations

import inspect

from .models import EndpointRequest, EndpointResponse


class IRuntimeEndpoint:
    @property
    def route(self) -> str:
        raise NotImplementedError

    def handle(self, request: EndpointRequest) -> EndpointResponse:
        raise NotImplementedError

    async def handle_async(self, request: EndpointRequest) -> EndpointResponse:
        result = self.handle(request)
        if inspect.isawaitable(result):
            result = await result
        return result


class BaseRuntimeEndpoint(IRuntimeEndpoint):
    def __init__(self, route: str):
        if not route or not route.startswith("/"):
            raise ValueError("route must start with '/'")
        self._route = route

    @property
    def route(self) -> str:
        return self._route

    def handle(self, request: EndpointRequest) -> EndpointResponse:
        return self.on_handle(request)

    async def handle_async(self, request: EndpointRequest) -> EndpointResponse:
        result = self.on_handle_async(request)
        if inspect.isawaitable(result):
            result = await result
        return result

    def on_handle(self, request: EndpointRequest) -> EndpointResponse:
        raise NotImplementedError

    async def on_handle_async(self, request: EndpointRequest) -> EndpointResponse:
        return self.on_handle(request)


class RuntimeEndpointController:
    def __init__(self):
        self._endpoints: dict[str, IRuntimeEndpoint] = {}

    def register(self, endpoint: IRuntimeEndpoint) -> bool:
        if endpoint.route in self._endpoints:
            return False
        self._endpoints[endpoint.route] = endpoint
        return True

    def unregister(self, route: str) -> bool:
        return self._endpoints.pop(route, None) is not None

    def resolve(self, route: str) -> IRuntimeEndpoint | None:
        return self._endpoints.get(route)

    def get_all(self) -> list[IRuntimeEndpoint]:
        return list(self._endpoints.values())
