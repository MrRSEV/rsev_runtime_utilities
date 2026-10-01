package de.rsev.utilities.events;

import java.util.concurrent.CompletableFuture;

/** Synchronous/asynchronous event listener contract. */
@FunctionalInterface
public interface IEventListener<T> {
    void listen(Object sender, T event);

    default CompletableFuture<Void> listenAsync(Object sender, T event) {
        return CompletableFuture.runAsync(() -> listen(sender, event));
    }
}
