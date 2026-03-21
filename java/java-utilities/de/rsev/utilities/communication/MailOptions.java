package de.rsev.utilities.communication;

/**
 * <summary>
 * Enthält alle Parameter für den Versand von E-Mails,
 * sowohl für einfachen lokalen Versand als auch für SMTP.
 * </summary>
 */
public class MailOptions {

    /** <summary>Die Absenderadresse der E-Mail.</summary> */
    private String mailFrom;

    /** <summary>Der Anzeigename des Absenders.</summary> */
    private String fromName;

    /** <summary>Die Empfängeradresse der E-Mail.</summary> */
    private String mailTo;

    /** <summary>Der Betreff der E-Mail.</summary> */
    private String subject;

    /** <summary>Der Inhalt der E-Mail.</summary> */
    private String mailBody;

    /** <summary>Der Hostname oder die IP-Adresse des SMTP-Servers.</summary> */
    private String smtpHost;

    /** <summary>Der Port des SMTP-Servers.</summary> */
    private int smtpPort = 587;

    /** <summary>Der Benutzername für die SMTP-Authentifizierung.</summary> */
    private String smtpUser;

    /** <summary>Das Passwort für die SMTP-Authentifizierung.</summary> */
    private String smtpPassword;

    /** <summary>Der Authentifizierungstyp (z. B. Login, Plain, OAuth).</summary> */
    private String authType = "Login";

    /** <summary>Aktiviert SSL-Verschlüsselung.</summary> */
    private boolean useSsl = false;

    /** <summary>Aktiviert TLS-Verschlüsselung.</summary> */
    private boolean useTls = true;

    /** <summary>Versandmethode: SMTP oder SendMail.</summary> */
    private String mailingMethod = "SMTP";

    /* -----------------------------
       Getter / Setter
       ----------------------------- */

    public String getMailFrom() { return mailFrom; }
    public void setMailFrom(String mailFrom) { this.mailFrom = mailFrom; }

    public String getFromName() { return fromName; }
    public void setFromName(String fromName) { this.fromName = fromName; }

    public String getMailTo() { return mailTo; }
    public void setMailTo(String mailTo) { this.mailTo = mailTo; }

    public String getSubject() { return subject; }
    public void setSubject(String subject) { this.subject = subject; }

    public String getMailBody() { return mailBody; }
    public void setMailBody(String mailBody) { this.mailBody = mailBody; }

    public String getSmtpHost() { return smtpHost; }
    public void setSmtpHost(String smtpHost) { this.smtpHost = smtpHost; }

    public int getSmtpPort() { return smtpPort; }
    public void setSmtpPort(int smtpPort) { this.smtpPort = smtpPort; }

    public String getSmtpUser() { return smtpUser; }
    public void setSmtpUser(String smtpUser) { this.smtpUser = smtpUser; }

    public String getSmtpPassword() { return smtpPassword; }
    public void setSmtpPassword(String smtpPassword) { this.smtpPassword = smtpPassword; }

    public String getAuthType() { return authType; }
    public void setAuthType(String authType) { this.authType = authType; }

    public boolean isUseSsl() { return useSsl; }
    public void setUseSsl(boolean useSsl) { this.useSsl = useSsl; }

    public boolean isUseTls() { return useTls; }
    public void setUseTls(boolean useTls) { this.useTls = useTls; }

    public String getMailingMethod() { return mailingMethod; }
    public void setMailingMethod(String mailingMethod) { this.mailingMethod = mailingMethod; }
}
