package de.rsev.utilities.plugin;

/**
 * <summary>
 * Abstrakte Basisklasse für Plugins.
 * Stellt einen standardisierten Plugin-Lifecycle bereit
 * und kapselt den Aktivierungszustand.
 * </summary>
 */
public abstract class BasePlugin implements IPlugin {

    protected boolean enabled;

    /* -----------------------------
       State Access Methods
       ----------------------------- */

    /**
     * <summary>
     * Gibt an, ob das Plugin aktuell aktiviert ist.
     * </summary>
     */
    @Override
    public boolean isEnabled() {
        return enabled;
    }

    /**
     * <summary>
     * Setzt den Aktivierungszustand des Plugins.
     * </summary>
     */
    @Override
    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
    }

    /* -----------------------------
       Lifecycle Methods
       ----------------------------- */

    /**
     * <summary>
     * Standard-Load-Logik des Plugins.
     * Aktiviert das Plugin.
     * </summary>
     */
    @Override
    public void onLoad() {
        enabled = true;
    }

    /**
     * <summary>
     * Standard-Reload-Logik.
     * Kann überschrieben werden, um interne Register,
     * Caches oder Zustände neu zu initialisieren.
     * </summary>
     */
    @Override
    public void onReload() {
        // Default: no-op
    }

    /**
     * <summary>
     * Standard-Unload-Logik des Plugins.
     * Deaktiviert das Plugin.
     * </summary>
     */
    @Override
    public void onUnload() {
        enabled = false;
    }
}
