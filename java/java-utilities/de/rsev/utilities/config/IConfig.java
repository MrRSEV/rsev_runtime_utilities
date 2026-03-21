package de.rsev.utilities.config;

import java.util.concurrent.CompletableFuture;

/**
 * <summary>
 * Definiert die grundlegenden Funktionen für ein universelles Konfigurationsobjekt.
 * Implementierungen können beliebige Formate wie JSON, YAML, CONF oder eigene
 * Strukturen verwenden. Das Interface ist bewusst minimal gehalten, um maximale
 * Flexibilität und Erweiterbarkeit zu gewährleisten.
 * </summary>
 */
public interface IConfig {

    /* =========================
       Load / Save (Sync)
       ========================= */

    /**
     * <summary>
     * Lädt die Konfiguration aus einer Datei oder Quelle.
     * </summary>
     */
    void load();

    /**
     * <summary>
     * Speichert die Konfiguration in eine Datei oder Quelle.
     * </summary>
     */
    void save(String path);

    /* =========================
       Load / Save (Async)
       ========================= */

    /**
     * <summary>
     * Lädt die Konfiguration asynchron.
     * </summary>
     */
    default CompletableFuture<Void> loadAsync() {
        return CompletableFuture.runAsync(this::load);
    }

    /**
     * <summary>
     * Speichert die Konfiguration asynchron.
     * </summary>
     */
    default CompletableFuture<Void> saveAsync(String path) {
        return CompletableFuture.runAsync(() -> save(path));
    }

    /* =========================
       Access
       ========================= */

    /**
     * <summary>
     * Liest einen Wert aus der Konfiguration anhand eines Schlüssels.
     * </summary>
     */
    <T> T get(String key, Class<T> type);

    /**
     * <summary>
     * Liest einen Wert asynchron aus der Konfiguration.
     * </summary>
     */
    default <T> CompletableFuture<T> getAsync(String key, Class<T> type) {
        return CompletableFuture.supplyAsync(() -> get(key, type));
    }
    
}
