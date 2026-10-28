use std::{
    io::{self, Read, Write},
    net::{TcpStream, ToSocketAddrs},
};
#[derive(Debug, Clone, Default)]
pub struct TlsOptions {
    pub enabled: bool,
    pub server_certificate: Option<String>,
    pub require_client_certificate: bool,
    pub allow_invalid_certificates: bool,
    pub target_host: Option<String>,
}
impl TlsOptions {
    pub fn none() -> Self {
        Self::default()
    }
    pub fn for_server(cert: impl Into<String>) -> Self {
        Self {
            enabled: true,
            server_certificate: Some(cert.into()),
            ..Default::default()
        }
    }
    pub fn for_client(host: impl Into<String>) -> Self {
        Self {
            enabled: true,
            target_host: Some(host.into()),
            ..Default::default()
        }
    }
}
pub struct TlsStreamFactory;
impl TlsStreamFactory {
    pub fn unsupported(options: &TlsOptions) -> io::Result<()> {
        if options.enabled {
            Err(io::Error::new(
                io::ErrorKind::Unsupported,
                "TLS requires a TLS backend; configure one at the application boundary",
            ))
        } else {
            Ok(())
        }
    }
}
pub struct TcpConnection {
    stream: TcpStream,
}
impl TcpConnection {
    pub fn send(&mut self, data: &[u8]) -> io::Result<()> {
        self.stream.write_all(data)
    }
    pub fn receive(&mut self) -> io::Result<Vec<u8>> {
        let mut b = Vec::new();
        self.stream.read_to_end(&mut b)?;
        Ok(b)
    }
    pub fn close(self) {
        drop(self)
    }
}
pub struct TcpTransport;
impl TcpTransport {
    pub fn connect<A: ToSocketAddrs>(
        address: A,
        options: &TlsOptions,
    ) -> io::Result<TcpConnection> {
        TlsStreamFactory::unsupported(options)?;
        Ok(TcpConnection {
            stream: TcpStream::connect(address)?,
        })
    }
}
