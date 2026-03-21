package de.rsev.utilities.communication.smtp;

import de.rsev.utilities.communication.MailOptions;
import java.util.concurrent.CompletableFuture;

/**
 * <summary>
 * Definiert den öffentlichen Vertrag für einen SMTP-Client.
 * Implementierungen kapseln das komplette SMTP-Protokoll,
 * inklusive Socket-Handling, Authentifizierung und Verschlüsselung.
 * </summary>
 */
public interface ISmtpClient {

    /* -----------------------------
       Send Methods
       ----------------------------- */

    /**
     * <summary>
     * Sendet eine E-Mail synchron über SMTP.
     * </summary>
     */
    void send(MailOptions options);

    /**
     * <summary>
     * Sendet eine E-Mail asynchron über SMTP.
     * </summary>
     */
    default CompletableFuture<Void> sendAsync(MailOptions options) {
        return CompletableFuture.runAsync(() -> send(options));
    }
}
