package de.rsev.utilities.events;

import java.util.List;

public interface IEventBus {
    <T> void registerEventRegistry(Class<T> eventType, IEventRegistry<T> registry);
    <T> IEventRegistry<T> getRegistry(Class<T> eventType, String eventName);
    <T> boolean hasRegistry(Class<T> eventType, String eventName);
    List<String> getAllEventNames();
}
