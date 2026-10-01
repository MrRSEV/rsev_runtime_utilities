package de.rsev.utilities.runtime;

import de.rsev.utilities.api.RuntimeApiHost;
import de.rsev.utilities.runtime.processes.RuntimeProcessController;

/**
 * <summary>
 * Abstrakte Basisklasse für Runtime-Kontexte.
 * Kapselt Basisverzeichnisse und Debug-Konfigurationen.
 * </summary>
 */
public abstract class BaseRuntimeContext implements IRuntimeContext {

    protected final String baseDirectory;
    protected boolean debug;
    protected final RuntimeProcessController processController;
    protected final RuntimeApiHost apiHost;

    /**
     * <summary>
     * Erstellt einen neuen Runtime-Kontext.
     * </summary>
     */
    protected BaseRuntimeContext(String baseDirectory) {
        this.baseDirectory = baseDirectory;
        this.processController = createProcessController();
        this.apiHost = createApiHost();
    }

    protected RuntimeProcessController createProcessController() {
        return new RuntimeProcessController();
    }

    protected RuntimeApiHost createApiHost() {
        return new RuntimeApiHost();
    }

    @Override
    public RuntimeProcessController getProcessController() {
        return processController;
    }

    @Override
    public RuntimeApiHost getApiHost() {
        return apiHost;
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
