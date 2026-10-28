use std::sync::{Arc, Mutex};

pub trait EventHandler<E>: Send + Sync {
    fn handle(&self, sender: &str, event: &E);
}
pub trait EventListener<E>: Send + Sync {
    fn listen(&self, sender: &str, event: &E);
}

pub struct EventRegistry<E> {
    pub event_name: String,
    handlers: Mutex<Vec<Arc<dyn EventHandler<E>>>>,
    listeners: Mutex<Vec<Arc<dyn EventListener<E>>>>,
}
impl<E> EventRegistry<E> {
    pub fn new(name: impl Into<String>) -> Self {
        let name = name.into();
        assert!(!name.trim().is_empty());
        Self {
            event_name: name,
            handlers: Mutex::new(Vec::new()),
            listeners: Mutex::new(Vec::new()),
        }
    }
    pub fn subscribe_handler(&self, handler: Arc<dyn EventHandler<E>>) {
        let mut h = self.handlers.lock().unwrap();
        if !h.iter().any(|x| Arc::ptr_eq(x, &handler)) {
            h.push(handler);
        }
    }
    pub fn subscribe_listener(&self, listener: Arc<dyn EventListener<E>>) {
        let mut l = self.listeners.lock().unwrap();
        if !l.iter().any(|x| Arc::ptr_eq(x, &listener)) {
            l.push(listener);
        }
    }
    pub fn unsubscribe_handler(&self, handler: &Arc<dyn EventHandler<E>>) {
        self.handlers
            .lock()
            .unwrap()
            .retain(|x| !Arc::ptr_eq(x, handler));
    }
    pub fn unsubscribe_listener(&self, listener: &Arc<dyn EventListener<E>>) {
        self.listeners
            .lock()
            .unwrap()
            .retain(|x| !Arc::ptr_eq(x, listener));
    }
    pub fn handler_count(&self) -> usize {
        self.handlers.lock().unwrap().len()
    }
    pub fn listener_count(&self) -> usize {
        self.listeners.lock().unwrap().len()
    }
    pub fn publish(&self, sender: &str, event: &E) {
        for h in self.handlers.lock().unwrap().clone() {
            let _ =
                std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| h.handle(sender, event)));
        }
        for l in self.listeners.lock().unwrap().clone() {
            let _ =
                std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| l.listen(sender, event)));
        }
    }
}

pub trait EventBus: Send + Sync {
    fn name(&self) -> &str;
    fn publish(&self, event: &str, payload: &[u8]);
}
pub struct BaseEventBus {
    pub bus_name: String,
}
impl BaseEventBus {
    pub fn new(name: impl Into<String>) -> Self {
        Self {
            bus_name: name.into(),
        }
    }
}
impl EventBus for BaseEventBus {
    fn name(&self) -> &str {
        &self.bus_name
    }
    fn publish(&self, _: &str, _: &[u8]) {}
}
