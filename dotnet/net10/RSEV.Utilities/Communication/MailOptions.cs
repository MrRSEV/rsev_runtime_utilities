using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Communication
{
    /// <summary>
    /// Enthält alle Parameter für den Versand von E-Mails,
    /// sowohl für einfachen lokalen Versand als auch für SMTP.
    /// </summary>
    public class MailOptions
    {
        /// <summary>
        /// Die Absenderadresse der E-Mail.
        /// </summary>
        public string MailFrom { get; set; }

        /// <summary>
        /// Der Anzeigename des Absenders.
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// Die Empfängeradresse der E-Mail.
        /// </summary>
        public string MailTo { get; set; }

        /// <summary>
        /// Der Betreff der E-Mail.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Der Inhalt der E-Mail.
        /// </summary>
        public string MailBody { get; set; }

        /// <summary>
        /// Der Hostname oder die IP-Adresse des SMTP-Servers.
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// Der Port des SMTP-Servers. Standard ist 587.
        /// </summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Der Benutzername für die SMTP-Authentifizierung.
        /// </summary>
        public string SmtpUser { get; set; }

        /// <summary>
        /// Das Passwort für die SMTP-Authentifizierung.
        /// </summary>
        public string SmtpPassword { get; set; }

        /// <summary>
        /// Der Authentifizierungstyp, z. B. "Login", "Plain" oder "OAuth".
        /// </summary>
        public string AuthType { get; set; } = "Login";

        /// <summary>
        /// Aktiviert SSL-Verschlüsselung für den SMTP-Versand.
        /// </summary>
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// Aktiviert TLS-Verschlüsselung für den SMTP-Versand.
        /// </summary>
        public bool UseTls { get; set; } = true;

        /// <summary>
        /// Gibt an, welche Versandmethode genutzt werden soll:
        /// "SMTP" oder "SendMail".
        /// </summary>
        public string MailingMethod { get; set; } = "SMTP";
    }
}