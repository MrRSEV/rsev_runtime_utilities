package de.rsev.utilities.command;

/**
 * <summary>
 * Abstrakter Spezial-Command zum gezielten Neuladen von Plugins.
 * Kapselt die Argument-Parsing-Logik und delegiert
 * die eigentliche Reload-Implementierung an Unterklassen.
 * </summary>
 */
public abstract class PluginReload extends BaseCommand {

    /**
     * <summary>
     * Erstellt einen neuen Plugin-Reload-Command.
     * </summary>
     */
    protected PluginReload(String name, String... aliases) {
        super(name, aliases);
    }

    /* -----------------------------
       Plugin Lifecycle Methods
       ----------------------------- */

    /**
     * <summary>
     * Führt die Neulade-Logik für ein spezifisches Plugin aus.
     * </summary>
     */
    protected abstract void reloadPlugin(String pluginName);

    /**
     * <summary>
     * Führt den Reload-Command aus und delegiert
     * an die spezifische Plugin-Reload-Implementierung.
     * </summary>
     */
    @Override
    public void execute(String[] args) {
        if (args.length == 0) return;
        reloadPlugin(args[0]);
    }
}
