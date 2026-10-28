use crate::networking::{TcpTransport, TlsOptions};
use std::{collections::HashMap, io};
#[derive(Debug, Clone, Default)]
pub struct RuntimeRequest {
    pub method: String,
    pub route: String,
    pub headers: HashMap<String, String>,
    pub body: Vec<u8>,
}
impl RuntimeRequest {
    pub fn to_bytes(&self) -> Vec<u8> {
        let mut x = format!("{} {} HTTP/1.1\r\n", self.method, self.route);
        for (k, v) in &self.headers {
            x.push_str(&format!("{k}: {v}\r\n"));
        }
        x.push_str(&format!("Content-Length: {}\r\n\r\n", self.body.len()));
        let mut b = x.into_bytes();
        b.extend(&self.body);
        b
    }
}
pub struct RuntimeRequestBuilder {
    request: RuntimeRequest,
}
impl RuntimeRequestBuilder {
    pub fn new() -> Self {
        Self {
            request: RuntimeRequest {
                method: "GET".into(),
                route: "/".into(),
                ..Default::default()
            },
        }
    }
    pub fn with_route(mut self, r: impl Into<String>) -> Self {
        self.request.route = r.into();
        self
    }
    pub fn with_method(mut self, m: impl Into<String>) -> Self {
        self.request.method = m.into();
        self
    }
    pub fn with_payload(mut self, p: impl Into<Vec<u8>>) -> Self {
        self.request.body = p.into();
        self
    }
    pub fn with_header(mut self, k: impl Into<String>, v: impl Into<String>) -> Self {
        self.request.headers.insert(k.into(), v.into());
        self
    }
    pub fn build(self) -> RuntimeRequest {
        self.request
    }
    pub fn build_bytes(self) -> Vec<u8> {
        self.build().to_bytes()
    }
}
#[derive(Debug, Clone, Default)]
pub struct RuntimeResponse {
    pub status_code: u16,
    pub headers: HashMap<String, String>,
    pub body: Vec<u8>,
}
impl RuntimeResponse {
    pub fn is_error(&self) -> bool {
        self.status_code >= 400
    }
}
pub struct RuntimeApiClient {
    pub address: String,
    pub tls: TlsOptions,
}
impl RuntimeApiClient {
    pub fn new(address: impl Into<String>) -> Self {
        Self {
            address: address.into(),
            tls: TlsOptions::none(),
        }
    }
    pub fn send(&self, request: RuntimeRequest) -> io::Result<RuntimeResponse> {
        let mut c = TcpTransport::connect(&self.address[..], &self.tls)?;
        c.send(&request.to_bytes())?;
        let raw = c.receive()?;
        parse_response(&raw)
    }
}
fn parse_response(raw: &[u8]) -> io::Result<RuntimeResponse> {
    let split = raw
        .windows(4)
        .position(|w| w == b"\r\n\r\n")
        .ok_or_else(|| io::Error::new(io::ErrorKind::InvalidData, "invalid HTTP response"))?;
    let head = String::from_utf8_lossy(&raw[..split]);
    let mut lines = head.lines();
    let status = lines
        .next()
        .and_then(|x| x.split_whitespace().nth(1))
        .and_then(|x| x.parse().ok())
        .unwrap_or(500);
    let mut headers = HashMap::new();
    for l in lines {
        if let Some((k, v)) = l.split_once(':') {
            headers.insert(k.trim().into(), v.trim().into());
        }
    }
    Ok(RuntimeResponse {
        status_code: status,
        headers,
        body: raw[split + 4..].to_vec(),
    })
}
