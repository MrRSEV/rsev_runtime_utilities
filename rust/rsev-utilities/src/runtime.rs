use crate::{
    api_builder::RuntimeApiHost,
    configuration::{BaseConfig, Config},
    loading::{AssemblyLoader, TypeDiscovery},
    logging::{Logger, SystemLog},
    messaging::MessageBusRegistry,
    plugins::PluginRegistry,
    processes::RuntimeProcessController,
};
use std::{
    collections::HashMap,
    sync::{Arc, Mutex},
};
pub trait RuntimeContext: Send + Sync {
    fn config(&self) -> Arc<Mutex<Box<dyn Config>>>;
    fn logger(&self) -> Arc<dyn Logger>;
    fn api_host(&self) -> &RuntimeApiHost;
    fn plugin_registry(&self) -> &PluginRegistry;
    fn message_bus_registry(&self) -> &MessageBusRegistry;
    fn process_controller(&self) -> &RuntimeProcessController;
    fn set(&self, key: String, value: String);
    fn get(&self, key: &str) -> Option<String>;
    fn is_running(&self) -> bool;
    fn set_running(&self, running: bool);
}
pub struct BaseRuntimeContext {
    pub config_store: Arc<Mutex<Box<dyn Config>>>,
    pub log: Arc<SystemLog>,
    pub loader: AssemblyLoader,
    pub discovery: TypeDiscovery,
    pub plugins: PluginRegistry,
    pub buses: MessageBusRegistry,
    pub processes: RuntimeProcessController,
    pub api: RuntimeApiHost,
    pub state: Mutex<HashMap<String, String>>,
    pub running: std::sync::atomic::AtomicBool,
}
impl Default for BaseRuntimeContext {
    fn default() -> Self {
        let log = Arc::new(SystemLog::default());
        Self {
            config_store: Arc::new(Mutex::new(Box::new(BaseConfig::default()))),
            api: RuntimeApiHost::new(Some(log.clone())),
            log,
            loader: Default::default(),
            discovery: Default::default(),
            plugins: Default::default(),
            buses: Default::default(),
            processes: Default::default(),
            state: Default::default(),
            running: std::sync::atomic::AtomicBool::new(false),
        }
    }
}
impl RuntimeContext for BaseRuntimeContext {
    fn config(&self) -> Arc<Mutex<Box<dyn Config>>> {
        self.config_store.clone()
    }
    fn logger(&self) -> Arc<dyn Logger> {
        self.log.clone()
    }
    fn api_host(&self) -> &RuntimeApiHost {
        &self.api
    }
    fn plugin_registry(&self) -> &PluginRegistry {
        &self.plugins
    }
    fn message_bus_registry(&self) -> &MessageBusRegistry {
        &self.buses
    }
    fn process_controller(&self) -> &RuntimeProcessController {
        &self.processes
    }
    fn set(&self, k: String, v: String) {
        self.state.lock().unwrap().insert(k, v);
    }
    fn get(&self, k: &str) -> Option<String> {
        self.state.lock().unwrap().get(k).cloned()
    }
    fn is_running(&self) -> bool {
        self.running.load(std::sync::atomic::Ordering::SeqCst)
    }
    fn set_running(&self, r: bool) {
        self.running.store(r, std::sync::atomic::Ordering::SeqCst)
    }
}
pub struct ShutdownSummary {
    pub successful_components: Vec<String>,
    pub errors: Vec<String>,
    pub completed: bool,
}
impl ShutdownSummary {
    pub fn new() -> Self {
        Self {
            successful_components: Vec::new(),
            errors: Vec::new(),
            completed: false,
        }
    }
    pub fn add_success(&mut self, n: impl Into<String>) {
        self.successful_components.push(n.into())
    }
    pub fn add_error(&mut self, e: impl Into<String>) {
        self.errors.push(e.into())
    }
    pub fn complete(&mut self) {
        self.completed = true
    }
}
pub trait OnStart {
    fn on_start(&self, _: &dyn RuntimeContext) -> Result<(), String> {
        Ok(())
    }
}
pub trait OnStop {
    fn on_stop(&self, _: &dyn RuntimeContext) -> Result<(), String> {
        Ok(())
    }
}
pub trait FirstRun: OnStart {}
