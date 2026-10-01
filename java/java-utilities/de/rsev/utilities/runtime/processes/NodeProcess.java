package de.rsev.utilities.runtime.processes;

import java.util.ArrayList;
import java.util.List;

public final class NodeProcess extends ExecutableProcess {
    public NodeProcess(String name, String script, List<String> arguments) {
        super(name, "node", buildArguments(script, arguments));
    }

    public NodeProcess(String name, String script) { this(name, script, List.of()); }
    private static List<String> buildArguments(String script, List<String> arguments) { List<String> values = new ArrayList<>(); values.add(script); if (arguments != null) values.addAll(arguments); return values; }
}
