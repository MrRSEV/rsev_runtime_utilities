package de.rsev.utilities.runtime;

import de.rsev.utilities.api.RuntimeApiHost;
import de.rsev.utilities.runtime.processes.RuntimeProcessController;

/**
 * <summary>
 * Definiert den Kontext der Runtime-Umgebung.
 * Stellt Basisinformationen wie Verzeichnisse
 * und Debug-Zustände bereit.
 * </summary>
 */
public interface IRuntimeContext {

    RuntimeProcessController getProcessController();

    RuntimeApiHost getApiHost();

    /**
     * <summary>
     * Gibt das Basisverzeichnis der Runtime zurück.
     * </summary>
     */
    String getBaseDirectory();

    /**
     * <summary>
     * Gibt an, ob der Debug-Modus aktiv ist.
     * </summary>
     */
    boolean isDebug();
}
