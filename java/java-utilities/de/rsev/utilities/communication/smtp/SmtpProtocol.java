package de.rsev.utilities.communication.smtp;

import java.io.IOException;

/**
 * <summary>
 * Implementiert den SMTP-Protokollablauf
 * (EHLO, MAIL FROM, RCPT TO, DATA, QUIT).
 * </summary>
 */
public class SmtpProtocol {

    private final SmtpConnection connection;

    public SmtpProtocol(SmtpConnection connection) {
        this.connection = connection;
    }

    /* -----------------------------
       SMTP Flow
       ----------------------------- */

    public void handshake(String clientName) throws IOException {
        expect(connection.readResponse());
        connection.sendLine("EHLO " + clientName);
        expect(connection.readResponse());
    }

    public void mailFrom(String from) throws IOException {
        connection.sendLine("MAIL FROM:<" + from + ">");
        expect(connection.readResponse());
    }

    public void rcptTo(String to) throws IOException {
        connection.sendLine("RCPT TO:<" + to + ">");
        expect(connection.readResponse());
    }

    public void data(String body) throws IOException {
        connection.sendLine("DATA");
        expect(connection.readResponse()); // 354

        connection.sendLine(body);
        connection.sendLine(".");
        expect(connection.readResponse());
    }

    public void quit() throws IOException {
        connection.sendLine("QUIT");
        connection.readResponse();
    }

    /* -----------------------------
       Helper
       ----------------------------- */

    private void expect(SmtpResponse response) {
        if (!response.isPositive()) {
            throw new RuntimeException("SMTP Fehler: " + response);
        }
    }
}
