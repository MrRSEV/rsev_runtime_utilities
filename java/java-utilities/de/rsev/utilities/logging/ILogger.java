package de.rsev.utilities.logging;

/**
 * <summary>
 * Zentrales Logger-Interface für das Utilities-Framework.
 * Definiert eine minimale, systemnahe Logging-API,
 * die plattformübergreifend einheitlich nutzbar ist.
 * </summary>
 */
public interface ILogger {

    /**
     * <summary>
     * Debug-Ausgabe.
     * </summary>
     */
    void debug(String message);

    /**
     * <summary>
     * Informations-Ausgabe.
     * </summary>
     */
    void info(String message);

    /**
     * <summary>
     * Warnungs-Ausgabe.
     * </summary>
     */
    void warn(String message);

    /**
     * <summary>
     * Fehler-Ausgabe.
     * </summary>
     */
    void error(String message, Throwable throwable);

    /**
     * <summary>
     * Kritischer Fehler.
     * </summary>
     */
    void critical(String message);
}
