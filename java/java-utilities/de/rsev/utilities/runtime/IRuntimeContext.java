package de.rsev.utilities.runtime;

/**
 * <summary>
 * Definiert den Kontext der Runtime-Umgebung.
 * Stellt Basisinformationen wie Verzeichnisse
 * und Debug-Zustände bereit.
 * </summary>
 */
public interface IRuntimeContext {

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
