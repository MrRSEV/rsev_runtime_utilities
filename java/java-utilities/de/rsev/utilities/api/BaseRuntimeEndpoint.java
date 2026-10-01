package de.rsev.utilities.api;

public abstract class BaseRuntimeEndpoint implements IRuntimeEndpoint {
    private final String route;

    protected BaseRuntimeEndpoint(String route) {
        if (route == null || !route.startsWith("/")) throw new IllegalArgumentException("route must start with '/'");
        this.route = route;
    }

    @Override public String getRoute() { return route; }
    @Override public EndpointResponse handle(EndpointRequest request) { return onHandle(request); }
    protected abstract EndpointResponse onHandle(EndpointRequest request);
}
