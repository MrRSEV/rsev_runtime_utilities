use crate::crypto::{hmac_sha256_public, constant_time_equal_public};
use std::time::{Duration, SystemTime, UNIX_EPOCH};

#[derive(Debug, Clone)]
pub struct AccessToken { pub value: String, pub expires_at: SystemTime }
impl AccessToken { pub fn is_expired(&self) -> bool { SystemTime::now() >= self.expires_at } }
#[derive(Debug, Clone)]
pub struct RefreshToken { pub value: String, pub expires_at: SystemTime }
impl RefreshToken { pub fn is_expired(&self) -> bool { SystemTime::now() >= self.expires_at } }
#[derive(Debug, Clone)]
pub struct SessionContext { pub subject: String, pub access_token: AccessToken, pub refresh_token: RefreshToken }
pub trait IAccessTokenProvider: Send + Sync { fn create(&self, subject: &str, lifetime: Duration) -> Result<AccessToken, String>; fn validate(&self, token: &AccessToken) -> bool; }

pub struct JwtTokenProvider { secret: Vec<u8> }
impl JwtTokenProvider { pub fn new(secret: &[u8]) -> Result<Self, String> { if secret.is_empty() { return Err("secret is required".into()); } Ok(Self { secret: secret.to_vec() }) } }
impl IAccessTokenProvider for JwtTokenProvider {
    fn create(&self, subject: &str, lifetime: Duration) -> Result<AccessToken, String> { if subject.is_empty() { return Err("subject is required".into()); } let expires = SystemTime::now().checked_add(lifetime).ok_or("invalid lifetime")?; let timestamp = expires.duration_since(UNIX_EPOCH).map_err(|_| "invalid time")?.as_secs(); let header = base64_url(b"{\"alg\":\"HS256\",\"typ\":\"JWT\"}"); let payload = base64_url(format!("{{\"sub\":\"{}\",\"exp\":{}}}", subject.replace('"', "\\\""), timestamp).as_bytes()); let unsigned = format!("{header}.{payload}"); let signature = base64_url(&hmac_sha256_public(&self.secret, unsigned.as_bytes())); Ok(AccessToken { value: format!("{unsigned}.{signature}"), expires_at: expires }) }
    fn validate(&self, token: &AccessToken) -> bool { if token.is_expired() { return false; } let parts: Vec<&str> = token.value.split('.').collect(); if parts.len() != 3 { return false; } let expected = hmac_sha256_public(&self.secret, format!("{}.{}", parts[0], parts[1]).as_bytes()); decode_base64_url(parts[2]).map(|value| constant_time_equal_public(&value, &expected)).unwrap_or(false) }
}
pub trait ISessionManager { fn create(&self, subject: &str) -> Result<SessionContext, String>; fn is_valid(&self, context: &SessionContext) -> bool; }
pub struct SessionManager<P: IAccessTokenProvider> { provider: P, access_lifetime: Duration, refresh_lifetime: Duration }
impl<P: IAccessTokenProvider> SessionManager<P> { pub fn new(provider: P) -> Self { Self { provider, access_lifetime: Duration::from_secs(3600), refresh_lifetime: Duration::from_secs(2_592_000) } } }
impl<P: IAccessTokenProvider> ISessionManager for SessionManager<P> { fn create(&self, subject: &str) -> Result<SessionContext, String> { let access = self.provider.create(subject, self.access_lifetime)?; let refresh = RefreshToken { value: base64_url(subject.as_bytes()), expires_at: SystemTime::now() + self.refresh_lifetime }; Ok(SessionContext { subject: subject.into(), access_token: access, refresh_token: refresh }) } fn is_valid(&self, context: &SessionContext) -> bool { self.provider.validate(&context.access_token) && !context.refresh_token.is_expired() } }

fn base64_url(value: &[u8]) -> String { const TABLE: &[u8] = b"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"; let mut output = String::new(); let mut index = 0; while index < value.len() { let a = value[index] as u32; let b = if index + 1 < value.len() { value[index + 1] as u32 } else { 0 }; let c = if index + 2 < value.len() { value[index + 2] as u32 } else { 0 }; output.push(TABLE[(a >> 2) as usize] as char); output.push(TABLE[((a << 4 | b >> 4) & 63) as usize] as char); if index + 1 < value.len() { output.push(TABLE[((b << 2 | c >> 6) & 63) as usize] as char); } if index + 2 < value.len() { output.push(TABLE[(c & 63) as usize] as char); } index += 3; } output }
fn decode_base64_url(value: &str) -> Option<Vec<u8>> { const TABLE: &[u8] = b"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_"; let mut buffer = 0u32; let mut bits = 0; let mut output = Vec::new(); for byte in value.bytes() { let digit = TABLE.iter().position(|candidate| *candidate == byte)? as u32; buffer = (buffer << 6) | digit; bits += 6; if bits >= 8 { bits -= 8; output.push((buffer >> bits) as u8); buffer &= (1 << bits) - 1; } } Some(output) }
