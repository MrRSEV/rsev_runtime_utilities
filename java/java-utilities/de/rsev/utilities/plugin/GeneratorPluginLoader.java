package de.rsev.utilities.plugin;

import java.util.Map;
import java.util.ServiceLoader;

public final class GeneratorPluginLoader {
    public int discover(ClassLoader classLoader, PluginRegistry registry, Map<String, Object> context) {
        int count = 0;
        for (IGeneratorPlugin plugin : ServiceLoader.load(IGeneratorPlugin.class, classLoader)) { registry.register(plugin); count++; }
        registry.generate(context);
        return count;
    }
}
