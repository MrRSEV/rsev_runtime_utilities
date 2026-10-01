package de.rsev.utilities.events;

import java.util.Objects;

/** Simple event metadata implementation. */
public final class Event<T> implements IEvent<T> {
    private final String name;
    private final String description;
    private final Class<T> eventArgsType;

    public Event(String name, Class<T> eventArgsType, String description) {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("Event name must not be blank");
        }
        this.name = name;
        this.eventArgsType = Objects.requireNonNull(eventArgsType, "eventArgsType");
        this.description = description == null ? "" : description;
    }

    public Event(String name, Class<T> eventArgsType) {
        this(name, eventArgsType, "");
    }

    @Override public String getName() { return name; }
    @Override public String getDescription() { return description; }
    @Override public Class<T> getEventArgsType() { return eventArgsType; }
}
