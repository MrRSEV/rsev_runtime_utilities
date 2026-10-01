package de.rsev.utilities.events;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ConcurrentHashMap;

/** Central event bus managing multiple typed registries. */
public class BaseEventBus implements IEventBus {
    private final ConcurrentHashMap<String, IEventRegistry<?>> registries = new ConcurrentHashMap<>();

    @Override
    public <T> void registerEventRegistry(Class<T> eventType, IEventRegistry<T> registry) {
        if (eventType == null || registry == null) return;
        registries.put(key(eventType, registry.getEventName()), registry);
    }

    @SuppressWarnings("unchecked")
    @Override
    public <T> IEventRegistry<T> getRegistry(Class<T> eventType, String eventName) {
        if (eventType == null || eventName == null) return null;
        return (IEventRegistry<T>) registries.get(key(eventType, eventName));
    }

    @Override
    public <T> boolean hasRegistry(Class<T> eventType, String eventName) {
        return getRegistry(eventType, eventName) != null;
    }

    @Override
    public List<String> getAllEventNames() {
        List<String> result = new ArrayList<>();
        registries.values().forEach(registry -> {
            if (!result.contains(registry.getEventName())) result.add(registry.getEventName());
        });
        return List.copyOf(result);
    }

    private static String key(Class<?> eventType, String eventName) {
        return eventType.getName() + "::" + eventName;
    }
}
