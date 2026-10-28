use std::time::{SystemTime, UNIX_EPOCH};

pub trait CryptoProvider: Send + Sync { fn algorithm(&self) -> &str; fn compute(&self, data: &[u8]) -> Vec<u8>; fn verify(&self, data: &[u8], signature: &[u8]) -> bool { constant_time_equal(&self.compute(data), signature) } }
pub trait ICryptoProvider: CryptoProvider {}
impl<T: CryptoProvider + ?Sized> ICryptoProvider for T {}

pub struct HamacProvider { key: Vec<u8> }
impl HamacProvider { pub fn new(key: &[u8]) -> Result<Self, String> { if key.is_empty() { return Err("key is required".into()); } Ok(Self { key: key.to_vec() }) } }
impl CryptoProvider for HamacProvider { fn algorithm(&self) -> &str { "HAMAC-SHA256" } fn compute(&self, data: &[u8]) -> Vec<u8> { hmac_sha256(&self.key, data) } }

#[derive(Debug, Default, Clone, Copy)]
pub struct Sha256Provider;
impl CryptoProvider for Sha256Provider { fn algorithm(&self) -> &str { "SHA-256" } fn compute(&self, data: &[u8]) -> Vec<u8> { sha256(data) } }

pub struct KeyManager;
impl KeyManager { pub fn generate(size: usize) -> Result<Vec<u8>, String> { if size < 16 { return Err("size must be at least 16".into()); } let seed = SystemTime::now().duration_since(UNIX_EPOCH).unwrap_or_default().as_nanos(); let mut output = Vec::with_capacity(size); for index in 0..size { output.push(((seed >> ((index % 16) * 8)) as u8) ^ (index as u8).wrapping_mul(31)); } Ok(output) } }

fn constant_time_equal(left: &[u8], right: &[u8]) -> bool { constant_time_equal_public(left, right) }
pub fn constant_time_equal_public(left: &[u8], right: &[u8]) -> bool { if left.len() != right.len() { return false; } left.iter().zip(right).fold(0u8, |value, (a, b)| value | (a ^ b)) == 0 }
fn hmac_sha256(key: &[u8], data: &[u8]) -> Vec<u8> { hmac_sha256_public(key, data) }
pub fn hmac_sha256_public(key: &[u8], data: &[u8]) -> Vec<u8> { let mut block = [0u8; 64]; if key.len() > 64 { block[..32].copy_from_slice(&sha256(key)); } else { block[..key.len()].copy_from_slice(key); } let mut inner = Vec::with_capacity(64 + data.len()); let mut outer = Vec::with_capacity(64 + 32); for byte in block { inner.push(byte ^ 0x36); outer.push(byte ^ 0x5c); } inner.extend_from_slice(data); outer.extend_from_slice(&sha256(&inner)); sha256(&outer) }

pub fn sha256(data: &[u8]) -> Vec<u8> {
    const K: [u32; 64] = [0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,0xd192e819,0xd6990624,0xf40e3585,0x106aa070,0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2];
    let mut h = [0x6a09e667u32,0xbb67ae85,0x3c6ef372,0xa54ff53a,0x510e527f,0x9b05688c,0x1f83d9ab,0x5be0cd19];
    let bit_len = (data.len() as u64) * 8; let mut padded = data.to_vec(); padded.push(0x80); while padded.len() % 64 != 56 { padded.push(0); } padded.extend_from_slice(&bit_len.to_be_bytes());
    for chunk in padded.chunks_exact(64) { let mut w = [0u32; 64]; for i in 0..16 { w[i] = u32::from_be_bytes([chunk[i*4],chunk[i*4+1],chunk[i*4+2],chunk[i*4+3]]); } for i in 16..64 { let s0 = w[i-15].rotate_right(7) ^ w[i-15].rotate_right(18) ^ (w[i-15] >> 3); let s1 = w[i-2].rotate_right(17) ^ w[i-2].rotate_right(19) ^ (w[i-2] >> 10); w[i] = w[i-16].wrapping_add(s0).wrapping_add(w[i-7]).wrapping_add(s1); }
        let (mut a,mut b,mut c,mut d,mut e,mut f,mut g,mut x) = (h[0],h[1],h[2],h[3],h[4],h[5],h[6],h[7]); for i in 0..64 { let s1=e.rotate_right(6)^e.rotate_right(11)^e.rotate_right(25); let ch=(e&f)^((!e)&g); let temp1=x.wrapping_add(s1).wrapping_add(ch).wrapping_add(K[i]).wrapping_add(w[i]); let s0=a.rotate_right(2)^a.rotate_right(13)^a.rotate_right(22); let maj=(a&b)^(a&c)^(b&c); let temp2=s0.wrapping_add(maj); x=g; g=f; f=e; e=d.wrapping_add(temp1); d=c; c=b; b=a; a=temp1.wrapping_add(temp2); } h[0]=h[0].wrapping_add(a);h[1]=h[1].wrapping_add(b);h[2]=h[2].wrapping_add(c);h[3]=h[3].wrapping_add(d);h[4]=h[4].wrapping_add(e);h[5]=h[5].wrapping_add(f);h[6]=h[6].wrapping_add(g);h[7]=h[7].wrapping_add(x);
    }
    h.iter().flat_map(|value| value.to_be_bytes()).collect()
}

#[cfg(test)]
mod tests {
    use super::{sha256, CryptoProvider, HamacProvider, Sha256Provider};

    #[test]
    fn sha256_matches_standard_vector() {
        let digest = sha256(b"abc");
        assert_eq!(digest.iter().map(|value| format!("{value:02x}")).collect::<String>(), "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
    }

    #[test]
    fn hamac_round_trip_verifies_in_constant_time() {
        let provider = HamacProvider::new(b"secret").unwrap();
        let signature = provider.compute(b"payload");
        assert!(provider.verify(b"payload", &signature));
        assert!(!provider.verify(b"different", &signature));
    }

    #[test]
    fn sha_provider_exposes_manifest_algorithm() {
        assert_eq!(Sha256Provider.algorithm(), "SHA-256");
    }
}
