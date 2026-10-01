package de.rsev.utilities.api;

import java.util.List;
import java.util.concurrent.ConcurrentHashMap;

public final class RuntimeEndpointController {
    private final ConcurrentHashMap<String, IRuntimeEndpoint> endpoints = new ConcurrentHashMap<>();
    public boolean register(IRuntimeEndpoint endpoint) { if (endpoint == null) throw new IllegalArgumentException("endpoint must not be null"); return endpoints.putIfAbsent(endpoint.getRoute(), endpoint) == null; }
    public boolean unregister(String route) { return endpoints.remove(route) != null; }
    public IRuntimeEndpoint resolve(String route) { return endpoints.get(route); }
    public List<IRuntimeEndpoint> getAll() { return List.copyOf(endpoints.values()); }
}
