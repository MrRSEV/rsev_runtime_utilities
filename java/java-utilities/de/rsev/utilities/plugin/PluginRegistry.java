package de.rsev.utilities.plugin;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;

public final class PluginRegistry {
    private final List<IGeneratorPlugin> plugins = new ArrayList<>();
    public synchronized void register(IGeneratorPlugin plugin) { if (plugins.stream().noneMatch(value -> value.name().equals(plugin.name()))) plugins.add(plugin); }
    public synchronized List<IGeneratorPlugin> all() { return plugins.stream().sorted(Comparator.comparing(IGeneratorPlugin::name)).toList(); }
    public void generate(Map<String, Object> context) { for (IGeneratorPlugin plugin : all()) plugin.generate(context); }
}
