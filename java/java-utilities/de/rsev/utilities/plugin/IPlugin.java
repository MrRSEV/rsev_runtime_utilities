package de.rsev.utilities.plugin;

public interface IPlugin {

    /**
     * @return true if the plugin is currently enabled
     */
    boolean isEnabled();

    /**
     * Enables or disables the plugin.
     */
    void setEnabled(boolean enabled);

    /**
     * Called once when the plugin is loaded for the first time.
     * Intended for full initialization.
     */
    void onLoad();

    /**
     * Called when the plugin should be reloaded without a full unload.
     *
     * <summary>
     * Used to clear and reinitialize registries and objects,
     * and to set specific default values if onLoad() should not be used
     * and a differentiated logic is required.
     * </summary>
     */
    void onReload();

    /**
     * Called when the plugin is fully unloaded.
     * Intended for cleanup and resource release.
     */
    void onUnload();
}
