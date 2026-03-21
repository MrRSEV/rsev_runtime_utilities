package de.rsev.utilities.communication.smtp;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.Base64;

/**
 * <summary>
 * Kapselt die SMTP-Authentifizierung.
 * Unterstützt AUTH LOGIN und AUTH PLAIN.
 * Diese Klasse enthält keinerlei Socket- oder Protokoll-Flow-Logik,
 * sondern arbeitet ausschließlich auf einer bestehenden SMTP-Verbindung.
 * </summary>
 */
public class SmtpAuthHandler {

    private final SmtpConnection connection;

    public SmtpAuthHandler(SmtpConnection connection) {
        this.connection = connection;
    }

    /* -----------------------------
       AUTH LOGIN
       ----------------------------- */

    /**
     * <summary>
     * Führt eine SMTP AUTH LOGIN Authentifizierung durch.
     * </summary>
     * <param name="username">SMTP Benutzername</param>
     * <param name="password">SMTP Passwort</param>
     */
    public void authLogin(String username, String password) throws IOException {
        connection.sendLine("AUTH LOGIN");
        expect(connection.readResponse(), 334);

        connection.sendLine(encodeBase64(username));
        expect(connection.readResponse(), 334);

        connection.sendLine(encodeBase64(password));
        expect(connection.readResponse(), 235);
    }

    /* -----------------------------
       AUTH PLAIN
       ----------------------------- */

    /**
     * <summary>
     * Führt eine SMTP AUTH PLAIN Authentifizierung durch.
     * </summary>
     * <param name="username">SMTP Benutzername</param>
     * <param name="password">SMTP Passwort</param>
     */
    public void authPlain(String username, String password) throws IOException {
        String payload = "\0" + username + "\0" + password;
        String encoded = encodeBase64(payload);

        connection.sendLine("AUTH PLAIN " + encoded);
        expect(connection.readResponse(), 235);
    }

    /* -----------------------------
       Helper
       ----------------------------- */

    /**
     * <summary>
     * Kodiert einen String in Base64 (UTF-8).
     * </summary>
     */
    private String encodeBase64(String value) {
        return Base64.getEncoder()
                .encodeToString(value.getBytes(StandardCharsets.UTF_8));
    }

    /**
     * <summary>
     * Validiert die SMTP-Antwort anhand eines erwarteten Codes.
     * </summary>
     */
    private void expect(SmtpResponse response, int expectedCode) {
        if (response.getCode() != expectedCode) {
            throw new RuntimeException(
                    "SMTP AUTH Fehler: Erwartet " + expectedCode +
                    ", erhalten " + response
            );
        }
    }
}
