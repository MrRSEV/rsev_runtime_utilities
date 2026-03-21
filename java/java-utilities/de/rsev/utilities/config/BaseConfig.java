package de.rsev.utilities.config;

/**
 * <summary>
 * Basisklasse für Konfigurationsimplementierungen.
 * Kapselt Ladezustand und Pfadinformationen.
 * Asynchrone Methoden werden über Default-Implementierungen
 * im Interface bereitgestellt.
 * </summary>
 */
public abstract class BaseConfig implements IConfig {

    protected volatile boolean loaded;
    protected final String path;

    protected BaseConfig(String path) {
        this.path = path;
    }

    /**
     * <summary>
     * Gibt den Pfad der Konfiguration zurück.
     * </summary>
     */
    public String getPath() {
        return path;
    }

    /**
     * <summary>
     * Gibt an, ob die Konfiguration geladen wurde.
     * </summary>
     */
    public boolean isLoaded() {
        return loaded;
    }
}
