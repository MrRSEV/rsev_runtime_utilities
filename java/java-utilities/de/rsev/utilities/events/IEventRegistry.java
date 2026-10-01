package de.rsev.utilities.events;

import java.util.List;
import java.util.concurrent.CompletableFuture;

public interface IEventRegistry<T> {
    String getEventName();
    void subscribe(IEventHandler<T> handler);
    void unsubscribe(IEventHandler<T> handler);
    void subscribeListener(IEventListener<T> listener);
    void unsubscribeListener(IEventListener<T> listener);
    List<IEventHandler<T>> getHandlers();
    List<IEventListener<T>> getListeners();
    void publish(Object sender, T event);
    CompletableFuture<Void> publishAsync(Object sender, T event);
}
