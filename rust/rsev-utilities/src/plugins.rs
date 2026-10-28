use crate::runtime::RuntimeContext;
use std::sync::{Arc, RwLock};
use std::collections::BTreeMap;
pub trait Plugin: Send + Sync {
    fn name(&self) -> &str;
    fn enable(&self, _: &dyn RuntimeContext) {}
    fn disable(&self, _: &dyn RuntimeContext) {}
}

pub trait IGeneratorPlugin: Send + Sync {
    fn name(&self) -> &str;
    fn version(&self) -> &str;
    fn generate(&self, context: &mut BTreeMap<String, String>);
}

#[derive(Default)]
pub struct GeneratorPluginRegistry { plugins: RwLock<BTreeMap<String, Arc<dyn IGeneratorPlugin>>> }
impl GeneratorPluginRegistry {
    pub fn register(&self, plugin: Arc<dyn IGeneratorPlugin>) -> Result<(), String> { let mut values = self.plugins.write().unwrap(); if values.contains_key(plugin.name()) { return Err(format!("plugin '{}' is already registered", plugin.name())); } values.insert(plugin.name().into(), plugin); Ok(()) }
    pub fn all(&self) -> Vec<Arc<dyn IGeneratorPlugin>> { self.plugins.read().unwrap().values().cloned().collect() }
    pub fn generate(&self, context: &mut BTreeMap<String, String>) { for plugin in self.all() { plugin.generate(context); } }
}
#[derive(Default)]
pub struct PluginRegistry {
    plugins: RwLock<Vec<Arc<dyn Plugin>>>,
}
impl PluginRegistry {
    pub fn register(&self, p: Arc<dyn Plugin>) {
        let mut x = self.plugins.write().unwrap();
        if !x.iter().any(|v| v.name() == p.name()) {
            x.push(p)
        }
    }
    pub fn all(&self) -> Vec<Arc<dyn Plugin>> {
        self.plugins.read().unwrap().clone()
    }
    pub fn enable_all(&self, c: &dyn RuntimeContext) {
        for p in self.all() {
            p.enable(c)
        }
    }
    pub fn disable_all(&self, c: &dyn RuntimeContext) {
        for p in self.all() {
            p.disable(c)
        }
    }
}
