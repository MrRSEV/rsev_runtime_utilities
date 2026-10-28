use std::{collections::HashMap, fs, io, str::FromStr};

pub trait Config: Send + Sync {
    fn load(&mut self, path: &str) -> io::Result<()>;
    fn save(&self, path: &str) -> io::Result<()>;
    fn get(&self, key: &str) -> Option<&str>;
    fn set(&mut self, key: String, value: String);
}

#[derive(Debug, Clone, Default)]
pub struct BaseConfig {
    pub values: HashMap<String, String>,
}
impl BaseConfig {
    pub fn get_value<T: FromStr>(&self, key: &str) -> Option<T> {
        self.get(key)?.parse().ok()
    }
}
impl Config for BaseConfig {
    fn load(&mut self, _path: &str) -> io::Result<()> {
        Ok(())
    }
    fn save(&self, _path: &str) -> io::Result<()> {
        Ok(())
    }
    fn get(&self, key: &str) -> Option<&str> {
        self.values.get(key).map(String::as_str)
    }
    fn set(&mut self, key: String, value: String) {
        self.values.insert(key, value);
    }
}

#[derive(Debug, Clone, Default)]
pub struct ConfConfig(pub BaseConfig);
#[derive(Debug, Clone, Default)]
pub struct JsonConfig(pub BaseConfig);
#[derive(Debug, Clone, Default)]
pub struct YamlConfig(pub BaseConfig);

fn load_lines(base: &mut BaseConfig, path: &str, separator: char) -> io::Result<()> {
    for line in fs::read_to_string(path)?.lines().map(str::trim) {
        if line.is_empty() || line.starts_with('#') {
            continue;
        }
        if let Some((k, v)) = line.split_once(separator) {
            base.set(
                k.trim().trim_matches('"').to_string(),
                v.trim().trim_matches(',').trim_matches('"').to_string(),
            );
        }
    }
    Ok(())
}
fn save_lines(base: &BaseConfig, path: &str, separator: char) -> io::Result<()> {
    fs::write(
        path,
        base.values
            .iter()
            .map(|(k, v)| format!("{k} {separator} {v}\n"))
            .collect::<String>(),
    )
}
macro_rules! config_impl {
    ($ty:ident, $sep:expr) => {
        impl Config for $ty {
            fn load(&mut self, p: &str) -> io::Result<()> {
                load_lines(&mut self.0, p, $sep)
            }
            fn save(&self, p: &str) -> io::Result<()> {
                save_lines(&self.0, p, $sep)
            }
            fn get(&self, k: &str) -> Option<&str> {
                self.0.get(k)
            }
            fn set(&mut self, k: String, v: String) {
                self.0.set(k, v)
            }
        }
    };
}
config_impl!(ConfConfig, '=');
config_impl!(JsonConfig, ':');
config_impl!(YamlConfig, ':');
