use crate::runtime::RuntimeContext;
use std::sync::{Arc, RwLock};

pub trait Command: Send + Sync {
    fn name(&self) -> &str;
    fn description(&self) -> &str {
        ""
    }
    fn execute(&self, context: &dyn RuntimeContext, arguments: &[String]) -> Result<(), String>;
}
pub struct BaseCommand {
    pub command_name: String,
    pub command_description: String,
}
impl BaseCommand {
    pub fn new(name: impl Into<String>, description: impl Into<String>) -> Self {
        Self {
            command_name: name.into(),
            command_description: description.into(),
        }
    }
}
impl Command for BaseCommand {
    fn name(&self) -> &str {
        &self.command_name
    }
    fn description(&self) -> &str {
        &self.command_description
    }
    fn execute(&self, _: &dyn RuntimeContext, _: &[String]) -> Result<(), String> {
        Ok(())
    }
}
#[derive(Default)]
pub struct CommandHandler {
    commands: RwLock<Vec<Arc<dyn Command>>>,
}
impl CommandHandler {
    pub fn register(&self, c: Arc<dyn Command>) {
        let mut x = self.commands.write().unwrap();
        if !x.iter().any(|v| v.name() == c.name()) {
            x.push(c)
        }
    }
    pub fn exists(&self, n: &str) -> bool {
        self.commands.read().unwrap().iter().any(|c| c.name() == n)
    }
    pub fn get(&self, n: &str) -> Option<Arc<dyn Command>> {
        self.commands
            .read()
            .unwrap()
            .iter()
            .find(|c| c.name() == n)
            .cloned()
    }
    pub fn execute(
        &self,
        n: &str,
        ctx: &dyn RuntimeContext,
        args: &[String],
    ) -> Result<bool, String> {
        match self.get(n) {
            Some(c) => {
                c.execute(ctx, args)?;
                Ok(true)
            }
            None => Ok(false),
        }
    }
    pub fn all(&self) -> Vec<Arc<dyn Command>> {
        self.commands.read().unwrap().clone()
    }
}
