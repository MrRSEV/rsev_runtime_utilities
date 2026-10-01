package de.rsev.utilities.events;

import java.util.concurrent.CompletableFuture;

/** Synchronous/asynchronous event handler contract. */
@FunctionalInterface
public interface IEventHandler<T> {
    void handle(Object sender, T event);

    default CompletableFuture<Void> handleAsync(Object sender, T event) {
        return CompletableFuture.runAsync(() -> handle(sender, event));
    }
}
