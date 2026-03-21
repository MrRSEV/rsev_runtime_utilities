package de.rsev.utilities.runtime;

/**
 * <summary>
 * Abstrakte Basisklasse für Runtime-Kontexte.
 * Kapselt Basisverzeichnisse und Debug-Konfigurationen.
 * </summary>
 */
public abstract class BaseRuntimeContext implements IRuntimeContext {

    protected final String baseDirectory;
    protected boolean debug;

    /**
     * <summary>
     * Erstellt einen neuen Runtime-Kontext.
     * </summary>
     */
    protected BaseRuntimeContext(String baseDirectory) {
        this.baseDirectory = baseDirectory;
    }

    /* -----------------------------
       Accessor Methods
       ----------------------------- */

    /**
     * <summary>
     * Gibt das Basisverzeichnis der Runtime zurück.
     * </summary>
     */
    @Override
    public String getBaseDirectory() {
        return baseDirectory;
    }

    /**
     * <summary>
     * Gibt an, ob der Debug-Modus aktiv ist.
     * </summary>
     */
    @Override
    public boolean isDebug() {
        return debug;
    }
}
