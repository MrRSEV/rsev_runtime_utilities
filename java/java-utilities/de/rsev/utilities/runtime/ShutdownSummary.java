package de.rsev.utilities.runtime;

import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;

/**
 * <summary>
 * Enthält eine Zusammenfassung des Shutdown-Prozesses.
 * Diese Klasse sammelt Informationen darüber, welche Komponenten und Prozesse
 * erfolgreich beendet wurden, welche Warnungen oder Fehler aufgetreten sind
 * und wie lange der Shutdown gedauert hat.
 * </summary>
 */
public abstract class ShutdownSummary {

    /**
     * <summary>
     * Zeitpunkt, an dem der Shutdown begonnen hat.
     * </summary>
     */
    private final Instant startTime = Instant.now();

    /**
     * <summary>
     * Zeitpunkt, an dem der Shutdown abgeschlossen wurde.
     * </summary>
     */
    private Instant endTime;

    /**
     * <summary>
     * Exit-Code des gesamten Shutdown-Vorgangs.
     * </summary>
     */
    private ExitCode exitCode = ExitCode.SUCCESS;

    /**
     * <summary>
     * Liste aller erfolgreich gestoppten Komponenten.
     * </summary>
     */
    private final List<String> successfulComponents = new ArrayList<>();

    /**
     * <summary>
     * Liste aller gestoppten Prozesse.
     * </summary>
     */
    private final List<String> stoppedProcesses = new ArrayList<>();

    /**
     * <summary>
     * Liste aller Warnungen, die während des Shutdowns aufgetreten sind.
     * </summary>
     */
    private final List<String> warnings = new ArrayList<>();

    /**
     * <summary>
     * Liste aller Fehler, die während des Shutdowns aufgetreten sind.
     * </summary>
     */
    private final List<String> errors = new ArrayList<>();

    /* -----------------------------
       Add / Mutator Methods
       ----------------------------- */

    /**
     * <summary>
     * Markiert eine Komponente als erfolgreich heruntergefahren.
     * </summary>
     */
    public void addSuccess(String componentName) {
        successfulComponents.add(componentName);
    }

    /**
     * <summary>
     * Registriert einen gestoppten Prozess.
     * </summary>
     */
    public void addProcess(String processName) {
        stoppedProcesses.add(processName);
    }

    /**
     * <summary>
     * Fügt eine Warnung zur Shutdown-Zusammenfassung hinzu.
     * </summary>
     */
    public void addWarning(String message) {
        warnings.add(message);
    }

    /**
     * <summary>
     * Fügt einen Fehler zur Shutdown-Zusammenfassung hinzu.
     * </summary>
     */
    public void addError(String message) {
        errors.add(message);
        exitCode = ExitCode.ERROR;
    }

    /**
     * <summary>
     * Setzt den Exit-Code explizit.
     * </summary>
     */
    public void setExitCode(ExitCode exitCode) {
        this.exitCode = exitCode;
    }

    /**
     * <summary>
     * Markiert den Shutdown als abgeschlossen.
     * </summary>
     */
    public void complete() {
        endTime = Instant.now();
    }

    /* -----------------------------
       Getter
       ----------------------------- */

    public Instant getStartTime() {
        return startTime;
    }

    public Instant getEndTime() {
        return endTime;
    }

    public ExitCode getExitCode() {
        return exitCode;
    }

    public Duration getDuration() {
        return endTime != null
                ? Duration.between(startTime, endTime)
                : Duration.ZERO;
    }

    /* -----------------------------
       Output / Logging
       ----------------------------- */

    /**
     * <summary>
     * Gibt eine menschenlesbare Zusammenfassung des Shutdown-Prozesses zurück.
     * </summary>
     */
    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();

        sb.append("Shutdown Summary\n");
        sb.append("----------------\n");
        sb.append("Start: ").append(startTime).append("\n");
        sb.append("End:   ").append(endTime).append("\n");
        sb.append("Duration: ")
          .append(String.format("%.2f", getDuration().toMillis() / 1000.0))
          .append(" seconds\n\n");

        if (!successfulComponents.isEmpty()) {
            sb.append("Successful Components:\n");
            successfulComponents.forEach(s -> sb.append("  ✔ ").append(s).append("\n"));
            sb.append("\n");
        }

        if (!stoppedProcesses.isEmpty()) {
            sb.append("Stopped Processes:\n");
            stoppedProcesses.forEach(p -> sb.append("  ◼ ").append(p).append("\n"));
            sb.append("\n");
        }

        if (!warnings.isEmpty()) {
            sb.append("Warnings:\n");
            warnings.forEach(w -> sb.append("  ⚠ ").append(w).append("\n"));
            sb.append("\n");
        }

        if (!errors.isEmpty()) {
            sb.append("Errors:\n");
            errors.forEach(e -> sb.append("  ✖ ").append(e).append("\n"));
        } else {
            sb.append("No errors occurred.\n");
        }

        return sb.toString();
    }
}
