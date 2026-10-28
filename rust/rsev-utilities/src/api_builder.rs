use crate::logging::Logger;
use std::{
    collections::HashMap,
    io::{Read, Write},
    net::{TcpListener, TcpStream},
    sync::{Arc, RwLock},
    thread,
};

#[derive(Debug, Clone, Default)]
pub struct EndpointRequest {
    pub method: String,
    pub route: String,
    pub headers: HashMap<String, String>,
    pub body: Vec<u8>,
}
#[derive(Debug, Clone, Default)]
pub struct EndpointResponse {
    pub status_code: u16,
    pub headers: HashMap<String, String>,
    pub body: Vec<u8>,
}
impl EndpointResponse {
    pub fn from_text(status: u16, text: impl Into<String>) -> Self {
        let body = text.into().into_bytes();
        let mut headers = HashMap::new();
        headers.insert("Content-Type".into(), "text/plain; charset=utf-8".into());
        Self {
            status_code: status,
            headers,
            body,
        }
    }
}
pub trait RuntimeEndpoint: Send + Sync {
    fn route(&self) -> &str;
    fn handle(&self, request: EndpointRequest) -> Result<EndpointResponse, String>;
}
pub struct BaseRuntimeEndpoint {
    pub endpoint_route: String,
}
impl BaseRuntimeEndpoint {
    pub fn new(r: impl Into<String>) -> Self {
        Self {
            endpoint_route: r.into(),
        }
    }
}
impl RuntimeEndpoint for BaseRuntimeEndpoint {
    fn route(&self) -> &str {
        &self.endpoint_route
    }
    fn handle(&self, _: EndpointRequest) -> Result<EndpointResponse, String> {
        Ok(EndpointResponse::from_text(200, "OK"))
    }
}
#[derive(Default)]
pub struct RuntimeEndpointController {
    endpoints: RwLock<HashMap<String, Arc<dyn RuntimeEndpoint>>>,
}
impl RuntimeEndpointController {
    pub fn register(&self, e: Arc<dyn RuntimeEndpoint>) -> bool {
        let mut x = self.endpoints.write().unwrap();
        if x.contains_key(e.route()) {
            false
        } else {
            x.insert(e.route().into(), e);
            true
        }
    }
    pub fn resolve(&self, r: &str) -> Option<Arc<dyn RuntimeEndpoint>> {
        self.endpoints.read().unwrap().get(r).cloned()
    }
}
pub struct RuntimeApiHost {
    controller: Arc<RuntimeEndpointController>,
    running: Arc<std::sync::atomic::AtomicBool>,
    logger: Option<Arc<dyn Logger>>,
}
impl RuntimeApiHost {
    pub fn new(logger: Option<Arc<dyn Logger>>) -> Self {
        Self {
            controller: Arc::new(Default::default()),
            running: Arc::new(std::sync::atomic::AtomicBool::new(false)),
            logger,
        }
    }
    pub fn register(&self, e: Arc<dyn RuntimeEndpoint>) -> bool {
        self.controller.register(e)
    }
    pub fn is_running(&self) -> bool {
        self.running.load(std::sync::atomic::Ordering::SeqCst)
    }
    pub fn stop(&self) {
        self.running
            .store(false, std::sync::atomic::Ordering::SeqCst)
    }
    pub fn start(&self, port: u16) -> std::io::Result<()> {
        let listener = TcpListener::bind(("0.0.0.0", port))?;
        self.running
            .store(true, std::sync::atomic::Ordering::SeqCst);
        if let Some(l) = &self.logger {
            l.info(&format!("RuntimeApiHost gestartet auf Port {port}"));
        }
        for stream in listener.incoming() {
            if !self.is_running() {
                break;
            }
            if let Ok(stream) = stream {
                let c = self.controller.clone();
                let l = self.logger.clone();
                thread::spawn(move || {
                    handle_client(stream, c, l);
                });
            }
        }
        Ok(())
    }
}
fn handle_client(
    mut stream: TcpStream,
    controller: Arc<RuntimeEndpointController>,
    logger: Option<Arc<dyn Logger>>,
) {
    let mut data = Vec::new();
    if stream.read_to_end(&mut data).is_err() {
        return;
    }
    let text = String::from_utf8_lossy(&data);
    let mut lines = text.split("\r\n");
    let first = lines.next().unwrap_or("");
    let parts: Vec<_> = first.split_whitespace().collect();
    let method = parts.first().unwrap_or(&"GET").to_string();
    let route = parts.get(1).unwrap_or(&"/").to_string();
    let mut headers = HashMap::new();
    for line in lines.by_ref() {
        if line.is_empty() {
            break;
        }
        if let Some((k, v)) = line.split_once(':') {
            headers.insert(k.trim().into(), v.trim().into());
        }
    }
    let req = EndpointRequest {
        method,
        route: route.clone(),
        headers,
        body: Vec::new(),
    };
    let response = match controller.resolve(&route) {
        None => EndpointResponse::from_text(404, "Not Found"),
        Some(e) => e
            .handle(req)
            .unwrap_or_else(|_| EndpointResponse::from_text(500, "Internal Server Error")),
    };
    let reason = match response.status_code {
        200 => "OK",
        404 => "Not Found",
        500 => "Internal Server Error",
        _ => "",
    };
    let mut out = format!(
        "HTTP/1.1 {} {}\r\nContent-Length: {}\r\n",
        response.status_code,
        reason,
        response.body.len()
    );
    for (k, v) in response.headers {
        out.push_str(&format!("{k}: {v}\r\n"));
    }
    out.push_str("\r\n");
    let _ = stream.write_all(out.as_bytes());
    let _ = stream.write_all(&response.body);
    if let Some(l) = logger {
        l.debug("Request verarbeitet");
    }
}
