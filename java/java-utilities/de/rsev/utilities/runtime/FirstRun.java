package de.rsev.utilities.runtime;

import de.rsev.utilities.plugin.*;

import java.io.IOException;
import java.nio.file.*;

/**
 * <summary>
 * Initialisierungs-Hilfsklasse für den ersten Programmstart.
 * Erstellt notwendige Verzeichnisstrukturen und
 * kopiert Plugin-Template-Dateien in das Base-Verzeichnis.
 * </summary>
 */
public final class FirstRun {

    private FirstRun() {}

    /* -----------------------------
       Initialization Methods
       ----------------------------- */

    /**
     * <summary>
     * Wird beim ersten Start der Anwendung ausgeführt.
     * </summary>
     */
    public static void onFirstRun(Path baseDir) throws IOException {
        Path pluginDir = baseDir.resolve("plugins");

        if (!Files.exists(pluginDir)) {
            Files.createDirectories(pluginDir);
            copyPluginTemplates(pluginDir);
        }
    }

    /**
     * <summary>
     * Kopiert Plugin-Template-Dateien in das Plugin-Verzeichnis.
     * </summary>
     */
    private static void copyPluginTemplates(Path pluginDir) throws IOException {
        copyResource("IPlugin.java", pluginDir.resolve("IPlugin.java"));
        copyResource("BasePlugin.java", pluginDir.resolve("BasePlugin.java"));
    }

    /**
     * <summary>
     * Kopiert eine eingebettete Template-Ressource in eine Zieldatei.
     * </summary>
     */
    private static void copyResource(String name, Path target) throws IOException {
        try (var in = FirstRun.class.getResourceAsStream("/templates/" + name)) {
            if (in == null) return;
            Files.copy(in, target, StandardCopyOption.REPLACE_EXISTING);
        }
    }
}
