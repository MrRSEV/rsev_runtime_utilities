use crate::runtime_client::{RuntimeApiClient, RuntimeRequest};
use std::{collections::VecDeque, io, sync::Mutex, thread, time::{Duration, Instant}};

#[derive(Debug, Clone, Default)]
pub struct ApiRequest { pub method: String, pub route: String, pub headers: std::collections::HashMap<String, String>, pub body: Vec<u8> }
#[derive(Debug, Clone, Default)]
pub struct ApiResponse { pub status_code: u16, pub headers: std::collections::HashMap<String, String>, pub body: Vec<u8> }
impl ApiResponse { pub fn is_success(&self) -> bool { (200..300).contains(&self.status_code) } }
pub trait IApiClient { fn send(&self, request: &ApiRequest) -> io::Result<ApiResponse>; }

#[derive(Debug, Clone)]
pub struct RetryPolicy { pub max_attempts: usize, pub delay: Duration }
impl Default for RetryPolicy { fn default() -> Self { Self { max_attempts: 3, delay: Duration::from_millis(200) } } }
#[derive(Debug)]
pub struct RateLimitPolicy { requests_per_window: usize, window: Duration, requests: Mutex<VecDeque<Instant>> }
impl RateLimitPolicy { pub fn new(requests_per_window: usize, window: Duration) -> Self { Self { requests_per_window, window, requests: Mutex::new(VecDeque::new()) } } pub fn try_acquire(&self) -> bool { let mut values = self.requests.lock().unwrap(); let cutoff = Instant::now() - self.window; while values.front().is_some_and(|value| *value <= cutoff) { values.pop_front(); } if values.len() >= self.requests_per_window { return false; } values.push_back(Instant::now()); true } }

pub struct HttpApiClient { client: RuntimeApiClient, retry_policy: RetryPolicy, rate_limit: RateLimitPolicy }
impl HttpApiClient { pub fn new(address: impl Into<String>) -> Self { Self { client: RuntimeApiClient::new(address), retry_policy: Default::default(), rate_limit: RateLimitPolicy::new(60, Duration::from_secs(60)) } } pub fn with_policies(mut self, retry_policy: RetryPolicy, rate_limit: RateLimitPolicy) -> Self { self.retry_policy = retry_policy; self.rate_limit = rate_limit; self } }
impl IApiClient for HttpApiClient {
    fn send(&self, request: &ApiRequest) -> io::Result<ApiResponse> {
        let attempts = self.retry_policy.max_attempts.max(1);
        for attempt in 0..attempts {
            if !self.rate_limit.try_acquire() { return Err(io::Error::new(io::ErrorKind::WouldBlock, "API rate limit reached")); }
            let runtime = RuntimeRequest { method: if request.method.is_empty() { "GET".into() } else { request.method.clone() }, route: request.route.clone(), headers: request.headers.clone(), body: request.body.clone() };
            let result = self.client.send(runtime)?;
            if result.status_code < 500 || attempt + 1 == attempts { return Ok(ApiResponse { status_code: result.status_code, headers: result.headers, body: result.body }); }
            thread::sleep(self.retry_policy.delay);
        }
        unreachable!()
    }
}
