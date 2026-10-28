use std::{
    fs::{create_dir_all, File, OpenOptions},
    io::Write,
    path::{Path, PathBuf},
    sync::Mutex,
    time::{SystemTime, UNIX_EPOCH},
};

pub trait Logger: Send + Sync {
    fn log(&self, level: LogLevel, message: &str);
    fn debug(&self, message: &str) {
        self.log(LogLevel::Debug, message);
    }
    fn info(&self, message: &str) {
        self.log(LogLevel::Info, message);
    }
    fn warning(&self, message: &str) {
        self.log(LogLevel::Warning, message);
    }
    fn error(&self, message: &str) {
        self.log(LogLevel::Error, message);
    }
    fn critical(&self, message: &str) {
        self.log(LogLevel::Critical, message);
    }
}

#[derive(Debug, Clone, Copy, PartialEq, Eq, PartialOrd, Ord)]
pub enum LogLevel {
    Debug,
    Info,
    Warning,
    Error,
    Critical,
}

pub struct SystemLog {
    writer: Mutex<Option<File>>,
    path: Mutex<Option<PathBuf>>,
}

impl SystemLog {
    pub fn new(directory: Option<impl AsRef<Path>>) -> Self {
        let log = Self {
            writer: Mutex::new(None),
            path: Mutex::new(None),
        };
        if let Some(dir) = directory {
            let _ = log.init(dir);
        }
        log
    }
    pub fn init(&self, directory: impl AsRef<Path>) -> std::io::Result<()> {
        let dir = directory.as_ref();
        create_dir_all(dir)?;
        let path = dir.join("rsev.log");
        if path.exists() {
            let stamp = SystemTime::now()
                .duration_since(UNIX_EPOCH)
                .unwrap_or_default()
                .as_secs();
            let _ = std::fs::rename(&path, dir.join(format!("{stamp}.log")));
        }
        *self.path.lock().unwrap() = Some(path.clone());
        *self.writer.lock().unwrap() =
            Some(OpenOptions::new().create(true).append(true).open(path)?);
        Ok(())
    }
    pub fn close(&self) {
        *self.writer.lock().unwrap() = None;
    }
}

impl Default for SystemLog {
    fn default() -> Self {
        Self::new(Option::<&Path>::None)
    }
}
impl Logger for SystemLog {
    fn log(&self, level: LogLevel, message: &str) {
        let line = format!("[{level:?}] {message}");
        println!("{line}");
        if let Some(writer) = self.writer.lock().unwrap().as_mut() {
            let _ = writeln!(writer, "{line}");
        }
    }
}
