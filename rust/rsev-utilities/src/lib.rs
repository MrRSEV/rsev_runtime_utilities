//! Rust implementation of the RSEV.Utilities design language.
//!
//! The module boundaries intentionally follow the .NET reference project. Rust
//! traits replace interfaces and composition (`Arc`/`Mutex`) replaces the
//! mutable inheritance-heavy parts of the original implementation.

pub mod api_builder;
pub mod api_tools;
pub mod commands;
pub mod connection;
pub mod communication;
pub mod configuration;
pub mod crypto;
pub mod controllers;
pub mod database;
pub mod events;
pub mod loading;
pub mod logging;
pub mod messaging;
pub mod networking;
pub mod plugins;
pub mod processes;
pub mod runtime;
pub mod runtime_client;
pub mod session;

pub use api_builder::{EndpointRequest, EndpointResponse, RuntimeApiHost};
pub use configuration::{BaseConfig, ConfConfig, Config, JsonConfig, YamlConfig};
pub use database::{ConnectionConfiguration, DatabaseAdapter, IDatabaseAdapter, MySqlAdapter, PostgreSqlAdapter, SqlServerAdapter, SqliteAdapter};
pub use logging::{LogLevel, Logger, SystemLog};
pub use runtime::{BaseRuntimeContext, RuntimeContext};

#[cfg(test)]
mod tests {
    use super::*;
    use crate::{
        events::{EventHandler, EventRegistry},
        runtime_client::RuntimeRequestBuilder,
    };
    use std::sync::{Arc, Mutex};

    struct Handler(Arc<Mutex<Vec<u32>>>);
    impl EventHandler<u32> for Handler {
        fn handle(&self, _: &str, event: &u32) {
            self.0.lock().unwrap().push(*event);
        }
    }

    #[test]
    fn event_registry_publishes_without_duplicate_subscriptions() {
        let values = Arc::new(Mutex::new(Vec::new()));
        let handler: Arc<dyn EventHandler<u32>> = Arc::new(Handler(values.clone()));
        let registry = EventRegistry::new("numbers");
        registry.subscribe_handler(handler.clone());
        registry.subscribe_handler(handler);
        registry.publish("test", &7);
        assert_eq!(&*values.lock().unwrap(), &[7]);
        assert_eq!(registry.handler_count(), 1);
    }

    #[test]
    fn request_builder_creates_http_payload() {
        let bytes = RuntimeRequestBuilder::new()
            .with_route("/health")
            .with_method("POST")
            .with_payload(b"ok".to_vec())
            .build_bytes();
        let text = String::from_utf8(bytes).unwrap();
        assert!(text.starts_with("POST /health HTTP/1.1\r\n"));
        assert!(text.ends_with("\r\nok"));
    }

    #[test]
    fn runtime_context_stores_global_state() {
        let context = BaseRuntimeContext::default();
        context.set("mode".into(), "test".into());
        assert_eq!(context.get("mode").as_deref(), Some("test"));
        assert!(!context.is_running());
    }

    #[test]
    fn endpoint_response_sets_content_type() {
        let response = EndpointResponse::from_text(200, "ok");
        assert_eq!(response.status_code, 200);
        assert_eq!(
            response.headers.get("Content-Type").unwrap(),
            "text/plain; charset=utf-8"
        );
    }
}
