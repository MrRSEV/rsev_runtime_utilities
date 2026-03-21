package de.rsev.utilities.runtime;

/**
 * <summary>
 * Definiert Exit-Codes für den Shutdown-Prozess.
 * </summary>
 */
public enum ExitCode {
    SUCCESS(0),
    WARNING(1),
    ERROR(2);

    private final int code;

    ExitCode(int code) {
        this.code = code;
    }

    public int getCode() {
        return code;
    }
}
