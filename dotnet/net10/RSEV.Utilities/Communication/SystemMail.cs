using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace RSEV.Utilities.Communication
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    namespace RSEV.Utilities.Communication
    {
        /// <summary>
        /// Universelle Mail-Utility-Klasse für einfachen Mailversand
        /// sowie SMTP-basierten Versand mit optionalen Credentials,
        /// Authentifizierung und Verschlüsselung.
        /// </summary>
        public class SystemMail
        {
            /// <summary>
            /// Optionales Parameterobjekt, das alle Mail- und SMTP-Informationen enthält.
            /// </summary>
            public virtual MailOptions Options { get; set; }

            /// <summary>
            /// Erstellt eine neue Instanz ohne vordefinierte Optionen.
            /// </summary>
            public SystemMail() { }

            /// <summary>
            /// Erstellt eine neue Instanz mit vordefinierten MailOptions.
            /// </summary>
            /// <param name="options">MailOptions-Objekt mit allen Parametern.</param>
            public SystemMail(MailOptions options)
            {
                Options = options;
            }

            // --------------------------------------------------------------------
            //  SENDMAIL (lokaler Versand, OS-unabhängig)
            // --------------------------------------------------------------------

            /// <summary>
            /// Sendet eine einfache E-Mail über den lokalen Maildienst (localhost).
            /// </summary>
            public virtual void SendMail(
                string mailFrom,
                string mailTo,
                string subject,
                string body,
                string fromName = null)
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(mailFrom, fromName),
                    Subject = subject,
                    Body = body
                };

                msg.To.Add(mailTo);

                using var client = new SmtpClient("localhost");
                client.Send(msg);
            }

            /// <summary>
            /// Sendet eine einfache E-Mail asynchron über den lokalen Maildienst (localhost).
            /// </summary>
            public virtual async Task SendMailAsync(
                string mailFrom,
                string mailTo,
                string subject,
                string body,
                string fromName = null)
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(mailFrom, fromName),
                    Subject = subject,
                    Body = body
                };

                msg.To.Add(mailTo);

                using var client = new SmtpClient("localhost");
                await client.SendMailAsync(msg);
            }

            // --------------------------------------------------------------------
            //  SENDMAIL SMTP (mit Credentials, SSL/TLS, Auth)
            // --------------------------------------------------------------------

            /// <summary>
            /// Sendet eine E-Mail über einen SMTP-Server mit optionaler
            /// Authentifizierung und Verschlüsselung.
            /// </summary>
            /// <param name="opt">MailOptions-Objekt mit allen SMTP-Parametern.</param>
            public virtual void SendMailSmtp(MailOptions opt)
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(opt.MailFrom, opt.FromName),
                    Subject = opt.Subject,
                    Body = opt.MailBody
                };

                msg.To.Add(opt.MailTo);

                using var client = new SmtpClient(opt.SmtpHost, opt.SmtpPort)
                {
                    EnableSsl = opt.UseSsl || opt.UseTls,
                    Credentials = new NetworkCredential(opt.SmtpUser, opt.SmtpPassword)
                };

                client.Send(msg);
            }

            /// <summary>
            /// Sendet eine E-Mail asynchron über einen SMTP-Server mit optionaler
            /// Authentifizierung und Verschlüsselung.
            /// </summary>
            /// <param name="opt">MailOptions-Objekt mit allen SMTP-Parametern.</param>
            public virtual async Task SendMailSmtpAsync(MailOptions opt)
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(opt.MailFrom, opt.FromName),
                    Subject = opt.Subject,
                    Body = opt.MailBody
                };

                msg.To.Add(opt.MailTo);

                using var client = new SmtpClient(opt.SmtpHost, opt.SmtpPort)
                {
                    EnableSsl = opt.UseSsl || opt.UseTls,
                    Credentials = new NetworkCredential(opt.SmtpUser, opt.SmtpPassword)
                };

                await client.SendMailAsync(msg);
            }

            // --------------------------------------------------------------------
            //  KOMFORTMETHODEN (nutzt Options aus dem Konstruktor)
            // --------------------------------------------------------------------

            /// <summary>
            /// Sendet eine E-Mail basierend auf den im Konstruktor übergebenen MailOptions.
            /// </summary>
            public virtual void Send()
            {
                if (Options == null)
                    return;

                if (Options.MailingMethod == "SMTP")
                    SendMailSmtp(Options);
                else
                    SendMail(
                        Options.MailFrom,
                        Options.MailTo,
                        Options.Subject,
                        Options.MailBody,
                        Options.FromName
                    );
            }

            /// <summary>
            /// Sendet eine E-Mail asynchron basierend auf den im Konstruktor übergebenen MailOptions.
            /// </summary>
            public virtual Task SendAsync()
            {
                if (Options == null)
                    return Task.CompletedTask;

                if (Options.MailingMethod == "SMTP")
                    return SendMailSmtpAsync(Options);

                return SendMailAsync(
                    Options.MailFrom,
                    Options.MailTo,
                    Options.Subject,
                    Options.MailBody,
                    Options.FromName
                );
            }
        }
    }
}