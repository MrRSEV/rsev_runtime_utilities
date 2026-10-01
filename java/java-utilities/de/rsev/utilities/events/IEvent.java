package de.rsev.utilities.events;

/** Metadata contract for an event definition. */
public interface IEvent<T> {
    String getName();
    String getDescription();
    Class<T> getEventArgsType();
}
