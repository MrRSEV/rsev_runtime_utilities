package de.rsev.utilities.communication.smtp;

import de.rsev.utilities.communication.MailOptions;

import java.io.IOException;

/**
 * <summary>
 * Native SMTP-Client-Implementierung ohne externe Dependencies.
 * Orchestriert das vollständige SMTP-Protokoll inklusive:
 * - EHLO / HELO
 * - optional STARTTLS
 * - optionale Authentifizierung (LOGIN / PLAIN)
 * - Mailversand (MAIL FROM / RCPT TO / DATA)
 *
 * Diese Klasse stellt den einzigen öffentlichen Einstiegspunkt
 * für SMTP-Kommunikation im Framework dar.
 * </summary>
 */
public class RawSmtpClient extends BaseSmtpClient implements ISmtpClient {

    /* -----------------------------
       Send Methods
       ----------------------------- */

    /**
     * <summary>
     * Sendet eine E-Mail synchron über SMTP.
     * </summary>
     */
    @Override
    public void send(MailOptions opt) {

        if (opt == null)
            throw new IllegalArgumentException("MailOptions dürfen nicht null sein");

        try (SmtpConnection connection =
                     new SmtpConnection(opt.getSmtpHost(), opt.getSmtpPort())) {

            // Protokoll & Helper
            SmtpProtocol protocol = new SmtpProtocol(connection);
            SmtpAuthHandler authHandler = new SmtpAuthHandler(connection);

            /* -----------------------------
               Initial Handshake
               ----------------------------- */

            protocol.handshake("localhost");

            /* -----------------------------
               STARTTLS (optional)
               ----------------------------- */

            if (opt.isUseTls()) {
                StartTlsHandler tlsHandler = new StartTlsHandler(connection);
                tlsHandler.startTls(opt.getSmtpHost());

                // RFC-konform: nach STARTTLS erneut EHLO
                protocol.handshake("localhost");
            }

            /* -----------------------------
               AUTH (optional)
               ----------------------------- */

            if (opt.getSmtpUser() != null && !opt.getSmtpUser().isEmpty()) {

                String authType = opt.getAuthType() != null
                        ? opt.getAuthType().toUpperCase()
                        : "LOGIN";

                switch (authType) {
                    case "PLAIN" ->
                            authHandler.authPlain(
                                    opt.getSmtpUser(),
                                    opt.getSmtpPassword()
                            );

                    case "LOGIN" ->
                            authHandler.authLogin(
                                    opt.getSmtpUser(),
                                    opt.getSmtpPassword()
                            );

                    default ->
                            throw new RuntimeException(
                                    "Nicht unterstützter SMTP Auth-Typ: " + authType
                            );
                }
            }

            /* -----------------------------
               Mail Transfer
               ----------------------------- */

            protocol.mailFrom(opt.getMailFrom());
            protocol.rcptTo(opt.getMailTo());
            protocol.data(buildMessage(opt));

            protocol.quit();

        } catch (IOException ex) {
            throw new RuntimeException("SMTP IO-Fehler", ex);
        }
    }

    /* -----------------------------
       Helper
       ----------------------------- */

    /**
     * <summary>
     * Baut den vollständigen Mail-Body inkl. Header.
     * Aktuell Plain-Text, erweiterbar für MIME / HTML.
     * </summary>
     */
    private String buildMessage(MailOptions opt) {

        StringBuilder sb = new StringBuilder();

        sb.append("From: ").append(opt.getMailFrom()).append("\r\n");
        sb.append("To: ").append(opt.getMailTo()).append("\r\n");
        sb.append("Subject: ").append(opt.getSubject()).append("\r\n");
        sb.append("Content-Type: text/plain; charset=UTF-8\r\n");
        sb.append("\r\n");
        sb.append(opt.getMailBody());

        return sb.toString();
    }
}
