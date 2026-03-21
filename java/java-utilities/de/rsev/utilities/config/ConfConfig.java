package de.rsev.utilities.config;

import java.util.HashMap;
import java.util.Map;

/**
 * <summary>
 * Klassische Linux-CONF-Konfigurationsimplementierung.
 * Unterstützt einfache key=value-Strukturen
 * ohne verschachtelte Hierarchien.
 * </summary>
 */
public class ConfConfig implements IConfig {

    private boolean loaded;
    private final String path;

    public ConfConfig(String path) {
        this.path = path;
    }

    @Override
    public void load() {
        loaded = true;
    }

    public void save(String path) {
        // save logic
    }

    public <T> T get(String key, Class<T> type) {
        return null;
    }

    @Override
    public void save() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'save'");
    }
}
