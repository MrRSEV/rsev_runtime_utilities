package de.rsev.utilities.communication.smtp;

/**
 * <summary>
 * Repräsentiert eine SMTP-Serverantwort.
 * Besteht aus Statuscode und Nachricht.
 * </summary>
 */
public class SmtpResponse {

    private final int code;
    private final String message;

    public SmtpResponse(int code, String message) {
        this.code = code;
        this.message = message;
    }

    public int getCode() {
        return code;
    }

    public String getMessage() {
        return message;
    }

    public boolean isPositive() {
        return code >= 200 && code < 400;
    }

    @Override
    public String toString() {
        return code + " " + message;
    }
}
