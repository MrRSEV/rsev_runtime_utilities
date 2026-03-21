package de.rsev.utilities.communication;

import de.rsev.utilities.communication.smtp.BaseSmtpClient;
import de.rsev.utilities.communication.smtp.RawSmtpClient;

import java.util.concurrent.CompletableFuture;

/**
 * <summary>
 * Universelle Mail-Utility-Klasse.
 * Dient als Komfort-Fassade für den Mailversand und delegiert
 * den eigentlichen SMTP-Transport an eine BaseSmtpClient-Implementierung.
 *
 * Diese Klasse enthält keine Protokoll- oder Transportlogik
 * und ist vollständig unabhängig von externen Mail-Frameworks.
 * </summary>
 */
public class SystemMail {

    /* -----------------------------
       Fields
       ----------------------------- */

    /**
     * <summary>
     * Optionales Parameterobjekt mit allen Mail- und SMTP-Informationen.
     * </summary>
     */
    protected MailOptions options;

    /**
     * <summary>
     * Interner SMTP-Client.
     * Standardmäßig RawSmtpClient.
     * </summary>
     */
    protected BaseSmtpClient smtpClient;

    /* -----------------------------
       Konstruktoren
       ----------------------------- */

    public SystemMail() {
        this.smtpClient = new RawSmtpClient();
    }

    public SystemMail(MailOptions options) {
        this.options = options;
        this.smtpClient = new RawSmtpClient();
    }

    public SystemMail(MailOptions options, BaseSmtpClient smtpClient) {
        this.options = options;
        this.smtpClient = smtpClient;
    }

    /* -----------------------------
       Configuration
       ----------------------------- */

    /**
     * <summary>
     * Setzt die MailOptions.
     * </summary>
     */
    public void setOptions(MailOptions options) {
        this.options = options;
    }

    /**
     * <summary>
     * Setzt eine benutzerdefinierte SMTP-Client-Implementierung.
     * </summary>
     */
    public void setSmtpClient(BaseSmtpClient smtpClient) {
        this.smtpClient = smtpClient;
    }

    /* -----------------------------
       Send (Sync)
       ----------------------------- */

    /**
     * <summary>
     * Sendet eine E-Mail synchron basierend auf den gesetzten MailOptions.
     * </summary>
     */
    public void send() {
        if (options == null)
            return;

        smtpClient.send(options);
    }

    /* -----------------------------
       Send (Async)
       ----------------------------- */

    /**
     * <summary>
     * Sendet eine E-Mail asynchron basierend auf den gesetzten MailOptions.
     * </summary>
     */
    public CompletableFuture<Void> sendAsync() {
        if (options == null)
            return CompletableFuture.completedFuture(null);

        return smtpClient.sendAsync(options);
    }
}
