package de.rsev.utilities.runtime.processing;

import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import de.rsev.utilities.runtime.ShutdownSummary;

/**
 * <summary>
 * Verwaltet externe oder interne Systemprozesse plattformübergreifend.
 * Unterstützt versteckte / daemonisierte Prozesse, PID-Registrierung,
 * Start mit Argumenten, Health-Checks sowie sauberes Beenden.
 * </summary>
 */
public interface IProcessManager {

    /**
     * <summary>
     * Startet den zugehörigen Prozess.
     * </summary>
     * @return True, wenn der Prozess erfolgreich gestartet wurde.
     */
    CompletableFuture<Boolean> startAsync();

    /**
     * <summary>
     * Startet einen Prozess mit Argumenten.
     * </summary>
     */
    CompletableFuture<Boolean> startAsync(List<String> arguments);

    /**
     * <summary>
     * Beendet den Prozess, falls er läuft.
     * </summary>
     */
    CompletableFuture<Boolean> stopAsync(ShutdownSummary summary);

    /**
     * <summary>
     * Gibt an, ob der Prozess aktuell läuft.
     * </summary>
     */
    boolean isRunning();

    /**
     * <summary>
     * Gibt die aktuelle Prozess-ID zurück, falls vorhanden.
     * </summary>
     */
    Integer getProcessId();

    /**
     * <summary>
     * Gibt den Namen oder Typ des Prozesses zurück.
     * </summary>
     */
    String getName();

    /**
     * <summary>
     * Optionaler Health-Check (z. B. HTTP Ping, Port-Test, IPC).
     * </summary>
     */
    CompletableFuture<Boolean> checkHealthAsync();

    /**
     * <summary>
     * Führt einen Systembefehl in einer Terminal- / Shell-Instanz aus.
     * </summary>
     */
    CompletableFuture<Integer> executeCommandAsync(String command, List<String> args);

    /**
     * <summary>
     * Wird aufgerufen, wenn der Prozess unerwartet beendet wird.
     * </summary>
     */
    void onUnexpectedExit(int exitCode);
}
