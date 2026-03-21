package de.rsev.utilities.runtime;

/**
 * <summary>
 * Registriert globale Cleanup-Handler für unerwartete
 * Programm- oder Prozessbeendigungen.
 * </summary>
 */
public final class UnexpectedExitHandler {

    private UnexpectedExitHandler() {}

    /* -----------------------------
       Registration Methods
       ----------------------------- */

    /**
     * <summary>
     * Registriert eine Cleanup-Logik, die beim JVM-Shutdown
     * automatisch ausgeführt wird.
     * </summary>
     */
    public static void register(Runnable cleanup) {
        Runtime.getRuntime().addShutdownHook(new Thread(cleanup));
    }
}
