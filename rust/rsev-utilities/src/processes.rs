use crate::logging::Logger;
use std::{
    collections::HashMap,
    io,
    process::{Child, Command, Stdio},
    sync::{Arc, Mutex},
};
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum SubprocessState {
    Created,
    Running,
    Stopped,
    Failed,
}
#[derive(Debug, Clone, Copy)]
pub enum ProcessLogLevel {
    Info,
    Warning,
    Error,
}
pub trait RuntimeSubprocess: Send + Sync {
    fn id(&self) -> &str;
    fn name(&self) -> &str;
    fn start(&mut self) -> io::Result<()>;
    fn stop(&mut self) -> io::Result<()>;
    fn state(&self) -> SubprocessState;
    fn health(&self) -> Option<bool> {
        Some(self.state() == SubprocessState::Running)
    }
}
pub struct CommandProcess {
    pub process_id: String,
    pub process_name: String,
    pub executable: String,
    pub arguments: Vec<String>,
    pub state: SubprocessState,
    child: Option<Child>,
    pub logger: Option<Arc<dyn Logger>>,
}
impl CommandProcess {
    pub fn new(
        id: impl Into<String>,
        name: impl Into<String>,
        exe: impl Into<String>,
        args: Vec<String>,
    ) -> Self {
        Self {
            process_id: id.into(),
            process_name: name.into(),
            executable: exe.into(),
            arguments: args,
            state: SubprocessState::Created,
            child: None,
            logger: None,
        }
    }
}
impl RuntimeSubprocess for CommandProcess {
    fn id(&self) -> &str {
        &self.process_id
    }
    fn name(&self) -> &str {
        &self.process_name
    }
    fn state(&self) -> SubprocessState {
        self.state
    }
    fn start(&mut self) -> io::Result<()> {
        let child = Command::new(&self.executable)
            .args(&self.arguments)
            .stdout(Stdio::piped())
            .stderr(Stdio::piped())
            .spawn()?;
        self.child = Some(child);
        self.state = SubprocessState::Running;
        Ok(())
    }
    fn stop(&mut self) -> io::Result<()> {
        if let Some(c) = &mut self.child {
            let _ = c.kill();
            let _ = c.wait();
        }
        self.child = None;
        self.state = SubprocessState::Stopped;
        Ok(())
    }
}
pub struct ShellProcess;
impl ShellProcess {
    pub fn new(
        id: impl Into<String>,
        name: impl Into<String>,
        command: impl Into<String>,
    ) -> CommandProcess {
        #[cfg(windows)]
        let exe = "powershell";
        #[cfg(not(windows))]
        let exe = "sh";
        #[cfg(windows)]
        let args = vec!["-Command".into(), command.into()];
        #[cfg(not(windows))]
        let args = vec!["-c".into(), command.into()];
        CommandProcess::new(id, name, exe, args)
    }
}
pub struct PythonProcess;
impl PythonProcess {
    pub fn new(
        id: impl Into<String>,
        name: impl Into<String>,
        script: impl Into<String>,
        args: Vec<String>,
    ) -> CommandProcess {
        let mut a = vec![script.into()];
        a.extend(args);
        CommandProcess::new(id, name, "python", a)
    }
}
pub struct NodeProcess;
impl NodeProcess {
    pub fn new(
        id: impl Into<String>,
        name: impl Into<String>,
        script: impl Into<String>,
        args: Vec<String>,
    ) -> CommandProcess {
        let mut a = vec![script.into()];
        a.extend(args);
        CommandProcess::new(id, name, "node", a)
    }
}
pub struct DotNetProcess;
impl DotNetProcess {
    pub fn new(
        id: impl Into<String>,
        name: impl Into<String>,
        dll: impl Into<String>,
        args: Vec<String>,
    ) -> CommandProcess {
        let mut a = vec![dll.into()];
        a.extend(args);
        CommandProcess::new(id, name, "dotnet", a)
    }
}
#[derive(Default)]
pub struct RuntimeProcessController {
    processes: Mutex<HashMap<String, Arc<Mutex<CommandProcess>>>>,
}
impl RuntimeProcessController {
    pub fn register(&self, p: CommandProcess) {
        self.processes
            .lock()
            .unwrap()
            .insert(p.process_id.clone(), Arc::new(Mutex::new(p)));
    }
    pub fn start(&self, id: &str) -> io::Result<bool> {
        match self.processes.lock().unwrap().get(id) {
            Some(p) => {
                p.lock().unwrap().start()?;
                Ok(true)
            }
            None => Ok(false),
        }
    }
    pub fn stop(&self, id: &str) -> io::Result<bool> {
        match self.processes.lock().unwrap().get(id) {
            Some(p) => {
                p.lock().unwrap().stop()?;
                Ok(true)
            }
            None => Ok(false),
        }
    }
    pub fn list(&self) -> Vec<(String, String, SubprocessState)> {
        self.processes
            .lock()
            .unwrap()
            .values()
            .map(|p| {
                let p = p.lock().unwrap();
                (p.process_id.clone(), p.process_name.clone(), p.state)
            })
            .collect()
    }
}
pub struct ExecutableResolver;
impl ExecutableResolver {
    pub fn resolve(environment_variable: &str, fallback: &str) -> String {
        std::env::var(environment_variable).unwrap_or_else(|_| fallback.into())
    }
}
