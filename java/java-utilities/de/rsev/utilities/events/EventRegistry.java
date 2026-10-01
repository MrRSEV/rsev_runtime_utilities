package de.rsev.utilities.events;

import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CopyOnWriteArrayList;

/** Thread-safe handler/listener registry for one event type. */
public class EventRegistry<T> implements IEventRegistry<T> {
    private final String eventName;
    private final CopyOnWriteArrayList<IEventHandler<T>> handlers = new CopyOnWriteArrayList<>();
    private final CopyOnWriteArrayList<IEventListener<T>> listeners = new CopyOnWriteArrayList<>();

    public EventRegistry(String eventName) {
        if (eventName == null || eventName.isBlank()) {
            throw new IllegalArgumentException("Event name must not be blank");
        }
        this.eventName = eventName;
    }

    @Override public String getEventName() { return eventName; }
    @Override public void subscribe(IEventHandler<T> handler) { if (handler != null) handlers.addIfAbsent(handler); }
    @Override public void unsubscribe(IEventHandler<T> handler) { handlers.remove(handler); }
    @Override public void subscribeListener(IEventListener<T> listener) { if (listener != null) listeners.addIfAbsent(listener); }
    @Override public void unsubscribeListener(IEventListener<T> listener) { listeners.remove(listener); }
    @Override public List<IEventHandler<T>> getHandlers() { return List.copyOf(handlers); }
    @Override public List<IEventListener<T>> getListeners() { return List.copyOf(listeners); }

    @Override
    public void publish(Object sender, T event) {
        for (IEventHandler<T> handler : handlers) {
            try { handler.handle(sender, event); } catch (RuntimeException ignored) { }
        }
        for (IEventListener<T> listener : listeners) {
            try { listener.listen(sender, event); } catch (RuntimeException ignored) { }
        }
    }

    @Override
    public CompletableFuture<Void> publishAsync(Object sender, T event) {
        List<CompletableFuture<Void>> futures = new java.util.ArrayList<>(handlers.stream()
            .map(handler -> safeAsync(() -> handler.handleAsync(sender, event)))
            .toList());
        futures.addAll(listeners.stream()
            .map(listener -> safeAsync(() -> listener.listenAsync(sender, event)))
            .toList());
        return CompletableFuture.allOf(futures.toArray(CompletableFuture[]::new));
    }

    private CompletableFuture<Void> safeAsync(java.util.function.Supplier<CompletableFuture<Void>> action) {
        try {
            return action.get().exceptionally(error -> null);
        } catch (RuntimeException error) {
            return CompletableFuture.completedFuture(null);
        }
    }
}
