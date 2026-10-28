use crate::runtime::RuntimeContext;
pub trait Controller: Send + Sync {
    fn name(&self) -> &str;
    fn initialize(&self, _: &dyn RuntimeContext) {}
    fn shutdown(&self, _: &dyn RuntimeContext) {}
}
pub struct BaseController {
    pub controller_name: String,
}
impl BaseController {
    pub fn new(n: impl Into<String>) -> Self {
        Self {
            controller_name: n.into(),
        }
    }
}
impl Controller for BaseController {
    fn name(&self) -> &str {
        &self.controller_name
    }
}
