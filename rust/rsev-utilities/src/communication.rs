use std::{
    io,
    io::{Read, Write},
    net::TcpStream,
};
#[derive(Debug, Clone)]
pub struct MailOptions {
    pub mail_from: String,
    pub from_name: String,
    pub mail_to: String,
    pub subject: String,
    pub mail_body: String,
    pub smtp_host: String,
    pub smtp_port: u16,
    pub smtp_user: String,
    pub smtp_password: String,
    pub auth_type: String,
    pub use_ssl: bool,
    pub use_tls: bool,
    pub mailing_method: String,
}
impl Default for MailOptions {
    fn default() -> Self {
        Self {
            mail_from: String::new(),
            from_name: String::new(),
            mail_to: String::new(),
            subject: String::new(),
            mail_body: String::new(),
            smtp_host: "localhost".into(),
            smtp_port: 587,
            smtp_user: String::new(),
            smtp_password: String::new(),
            auth_type: "Login".into(),
            use_ssl: false,
            use_tls: true,
            mailing_method: "SMTP".into(),
        }
    }
}
pub struct SystemMail {
    pub options: Option<MailOptions>,
}
impl SystemMail {
    pub fn new(options: Option<MailOptions>) -> Self {
        Self { options }
    }
    pub fn send_smtp(&self, opt: &MailOptions) -> io::Result<()> {
        let mut stream = TcpStream::connect((opt.smtp_host.as_str(), opt.smtp_port))?;
        let mut response = [0; 1024];
        let _ = stream.read(&mut response);
        smtp_line(&mut stream, &format!("MAIL FROM:<{}>\r\n", opt.mail_from))?;
        smtp_line(&mut stream, &format!("RCPT TO:<{}>\r\n", opt.mail_to))?;
        smtp_line(&mut stream, "DATA\r\n")?;
        smtp_line(
            &mut stream,
            &format!(
                "Subject: {}\r\nFrom: {}\r\nTo: {}\r\n\r\n{}\r\n.\r\n",
                opt.subject, opt.mail_from, opt.mail_to, opt.mail_body
            ),
        )?;
        smtp_line(&mut stream, "QUIT\r\n")?;
        Ok(())
    }
    pub fn send(&self) -> io::Result<()> {
        match &self.options {
            Some(o) => self.send_smtp(o),
            None => Ok(()),
        }
    }
}
fn smtp_line(stream: &mut TcpStream, line: &str) -> io::Result<()> {
    stream.write_all(line.as_bytes())?;
    let mut b = [0; 512];
    let _ = stream.read(&mut b);
    Ok(())
}
