use crate::database::{ConnectionConfiguration, DatabaseAdapter, DatabaseConnection};
use std::{collections::VecDeque, sync::{Arc, Mutex}};

pub struct ConnectionFactory { adapter: Arc<dyn DatabaseAdapter> }
impl ConnectionFactory {
    pub fn new(adapter: Arc<dyn DatabaseAdapter>) -> Self { Self { adapter } }
    pub fn create(&self, configuration: &ConnectionConfiguration) -> Result<DatabaseConnection, String> { self.adapter.create_connection(configuration) }
}

pub struct ConnectionPool { connections: Mutex<VecDeque<DatabaseConnection>>, maximum_size: usize }
impl ConnectionPool {
    pub fn new(maximum_size: usize) -> Result<Self, String> { if maximum_size == 0 { return Err("maximum_size must be positive".into()); } Ok(Self { connections: Mutex::new(VecDeque::new()), maximum_size }) }
    pub fn rent<F: FnOnce() -> Result<DatabaseConnection, String>>(&self, factory: F) -> Result<DatabaseConnection, String> { if let Some(value) = self.connections.lock().unwrap().pop_back() { Ok(value) } else { factory() } }
    pub fn give_back(&self, connection: DatabaseConnection) -> bool { let mut values = self.connections.lock().unwrap(); if values.len() >= self.maximum_size { return false; } values.push_back(connection); true }
    pub fn clear(&self) { self.connections.lock().unwrap().clear(); }
}

pub trait IConnectionManager: Send + Sync { fn open(&self, configuration: &ConnectionConfiguration) -> Result<DatabaseConnection, String>; fn give_back(&self, connection: DatabaseConnection); }
pub struct ConnectionManager { factory: ConnectionFactory, pool: ConnectionPool }
impl ConnectionManager { pub fn new(factory: ConnectionFactory, configuration: &ConnectionConfiguration) -> Result<Self, String> { configuration.validate()?; Ok(Self { factory, pool: ConnectionPool::new(configuration.maximum_pool_size)? }) } }
impl IConnectionManager for ConnectionManager { fn open(&self, configuration: &ConnectionConfiguration) -> Result<DatabaseConnection, String> { self.pool.rent(|| self.factory.create(configuration)) } fn give_back(&self, connection: DatabaseConnection) { self.pool.give_back(connection); } }
