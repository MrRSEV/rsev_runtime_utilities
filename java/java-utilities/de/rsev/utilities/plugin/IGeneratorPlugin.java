package de.rsev.utilities.plugin;

import java.util.Map;

public interface IGeneratorPlugin {
    String name();
    String version();
    void generate(Map<String, Object> context);
}
