use crate::runtime::RuntimeContext;
use std::sync::{Arc, RwLock};
pub trait MessageBus: Send + Sync {
    fn name(&self) -> &str;
    fn description(&self) -> &str {
        ""
    }
    fn initialize(&self, _: &dyn RuntimeContext) {}
    fn shutdown(&self, _: &dyn RuntimeContext) {}
}
pub struct BaseMessageBus {
    pub bus_name: String,
    pub bus_description: String,
}
impl BaseMessageBus {
    pub fn new(n: impl Into<String>, d: impl Into<String>) -> Self {
        Self {
            bus_name: n.into(),
            bus_description: d.into(),
        }
    }
}
impl MessageBus for BaseMessageBus {
    fn name(&self) -> &str {
        &self.bus_name
    }
    fn description(&self) -> &str {
        &self.bus_description
    }
}
#[derive(Default)]
pub struct MessageBusRegistry {
    buses: RwLock<Vec<Arc<dyn MessageBus>>>,
}
impl MessageBusRegistry {
    pub fn register(&self, b: Arc<dyn MessageBus>) {
        let mut x = self.buses.write().unwrap();
        if !x.iter().any(|v| v.name() == b.name()) {
            x.push(b)
        }
    }
    pub fn get(&self, n: &str) -> Option<Arc<dyn MessageBus>> {
        self.buses
            .read()
            .unwrap()
            .iter()
            .find(|b| b.name() == n)
            .cloned()
    }
    pub fn exists(&self, n: &str) -> bool {
        self.get(n).is_some()
    }
    pub fn all(&self) -> Vec<Arc<dyn MessageBus>> {
        self.buses.read().unwrap().clone()
    }
    pub fn initialize_all(&self, c: &dyn RuntimeContext) {
        for b in self.all() {
            b.initialize(c)
        }
    }
    pub fn shutdown_all(&self, c: &dyn RuntimeContext) {
        for b in self.all() {
            b.shutdown(c)
        }
    }
}
