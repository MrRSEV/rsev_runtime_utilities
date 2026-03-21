package de.rsev.utilities.runtime.processing;

import java.io.IOException;
import java.util.List;
import java.util.concurrent.CompletableFuture;

/**
 * <summary>
 * Basisklasse für plattformabhängige ProcessManager.
 * Kapselt PID-Handling, Hidden-Process-Logik und Exit-Überwachung.
 * </summary>
 */
public abstract class BaseProcessManager implements IProcessManager {

    protected Process process;
    protected String name;
    protected Integer pid;

    /**
     * <summary>
     * Erstellt einen neuen ProcessManager mit Namen.
     * </summary>
     */
    protected BaseProcessManager(String name) {
        this.name = name;
    }

    /* -----------------------------
       State Access Methods
       ----------------------------- */

    /**
     * <summary>
     * Gibt an, ob der verwaltete Prozess aktuell läuft.
     * </summary>
     */
    @Override
    public boolean isRunning() {
        return process != null && process.isAlive();
    }

    /**
     * <summary>
     * Gibt die Prozess-ID des verwalteten Prozesses zurück.
     * </summary>
     */
    @Override
    public Integer getProcessId() {
        return pid;
    }

    /**
     * <summary>
     * Gibt den logischen Namen des Prozesses zurück.
     * </summary>
     */
    @Override
    public String getName() {
        return name;
    }

    /* -----------------------------
       Process Handling Methods
       ----------------------------- */

    /**
     * <summary>
     * Registriert einen gestarteten Prozess und überwacht
     * dessen unerwartetes Beenden.
     * </summary>
     */
    protected void registerProcess(Process p) {
        this.process = p;
        this.pid = (int) p.pid();

        p.onExit().thenAccept(proc -> {
            pid = null;
            onUnexpectedExit(proc.exitValue());
        });
    }

    /**
     * <summary>
     * Führt einen System-Command asynchron aus.
     * </summary>
     */
    @Override
    public CompletableFuture<Integer> executeCommandAsync(String command, List<String> args) {
        return CompletableFuture.supplyAsync(() -> {
            try {
                ProcessBuilder pb = new ProcessBuilder();
                pb.command(command);
                pb.command().addAll(args);
                pb.redirectErrorStream(true);

                Process p = pb.start();
                return p.waitFor();
            } catch (IOException | InterruptedException e) {
                throw new RuntimeException(e);
            }
        });
    }

    /**
     * <summary>
     * Wird aufgerufen, wenn ein Prozess unerwartet beendet wird.
     * Kann von plattformspezifischen Implementierungen überschrieben werden.
     * </summary>
     */
    @Override
    public void onUnexpectedExit(int exitCode) {
        // Default: no-op
    }
}
