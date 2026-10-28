use std::fmt;

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct ConnectionConfiguration {
    pub provider: String,
    pub connection_string: String,
    pub username: String,
    pub password: String,
    pub minimum_pool_size: usize,
    pub maximum_pool_size: usize,
}
impl ConnectionConfiguration {
    pub fn new(provider: impl Into<String>, connection_string: impl Into<String>) -> Result<Self, String> {
        let value = Self { provider: provider.into(), connection_string: connection_string.into(), username: String::new(), password: String::new(), minimum_pool_size: 1, maximum_pool_size: 10 };
        value.validate()?;
        Ok(value)
    }
    pub fn validate(&self) -> Result<(), String> {
        if self.provider.trim().is_empty() || self.connection_string.trim().is_empty() { return Err("provider and connection_string are required".into()); }
        if self.minimum_pool_size > self.maximum_pool_size || self.maximum_pool_size == 0 { return Err("invalid pool bounds".into()); }
        Ok(())
    }
}

#[derive(Debug, Clone, PartialEq, Eq)]
pub struct DatabaseConnection { pub provider: String, pub connection_string: String }

pub trait DatabaseAdapter: Send + Sync {
    fn provider(&self) -> &str;
    fn create_connection(&self, configuration: &ConnectionConfiguration) -> Result<DatabaseConnection, String>;
}
pub trait IDatabaseAdapter: DatabaseAdapter {}
impl<T: DatabaseAdapter + ?Sized> IDatabaseAdapter for T {}

macro_rules! adapter {
    ($name:ident, $provider:literal) => {
        #[derive(Debug, Default, Clone, Copy)]
        pub struct $name;
        impl DatabaseAdapter for $name {
            fn provider(&self) -> &str { $provider }
            fn create_connection(&self, configuration: &ConnectionConfiguration) -> Result<DatabaseConnection, String> {
                configuration.validate()?;
                Ok(DatabaseConnection { provider: self.provider().into(), connection_string: configuration.connection_string.clone() })
            }
        }
    };
}
adapter!(MySqlAdapter, "mysql");
adapter!(SqlServerAdapter, "mssql");
adapter!(PostgreSqlAdapter, "postgresql");
adapter!(SqliteAdapter, "sqlite");

impl fmt::Display for DatabaseConnection { fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result { write!(f, "{}:{}", self.provider, self.connection_string) } }
