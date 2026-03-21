package de.rsev.utilities.communication.smtp;

import javax.net.ssl.SSLSocket;
import javax.net.ssl.SSLSocketFactory;
import java.io.IOException;
import java.net.Socket;

/**
 * <summary>
 * Implementiert das SMTP STARTTLS Kommando gemäß RFC 3207.
 * Diese Klasse ist verantwortlich für das Upgrade einer bestehenden
 * unverschlüsselten SMTP-Verbindung auf TLS.
 *
 * Der Handler ersetzt den zugrunde liegenden Socket der bestehenden
 * SmtpConnection durch einen SSLSocket.
 * </summary>
 */
public class StartTlsHandler {

    private final SmtpConnection connection;

    public StartTlsHandler(SmtpConnection connection) {
        this.connection = connection;
    }

    /* -----------------------------
       STARTTLS
       ----------------------------- */

    /**
     * <summary>
     * Führt ein STARTTLS-Upgrade auf der bestehenden SMTP-Verbindung durch.
     * </summary>
     * <param name="host">SMTP-Hostname (für TLS-Handshakes)</param>
     */
    public void startTls(String host) throws IOException {

        // STARTTLS Kommando senden
        connection.sendLine("STARTTLS");
        SmtpResponse response = connection.readResponse();

        if (response.getCode() != 220) {
            throw new RuntimeException(
                    "STARTTLS nicht unterstützt oder fehlgeschlagen: " + response
            );
        }

        // Bestehenden Socket upgraden
        upgradeSocket(host);
    }

    /* -----------------------------
       Socket Upgrade
       ----------------------------- */

    /**
     * <summary>
     * Ersetzt den bestehenden Socket durch einen SSLSocket
     * und initialisiert neue Reader/Writer auf der TLS-Verbindung.
     * </summary>
     */
    private void upgradeSocket(String host) throws IOException {

        Socket plainSocket = connection.socket;

        SSLSocketFactory factory =
                (SSLSocketFactory) SSLSocketFactory.getDefault();

        SSLSocket sslSocket = (SSLSocket) factory.createSocket(
                plainSocket,
                host,
                plainSocket.getPort(),
                true
        );

        sslSocket.startHandshake();

        connection.replaceSocket(sslSocket);
    }
}
