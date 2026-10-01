package de.rsev.utilities.runtime.processes;

import java.util.ArrayList;
import java.util.List;

public final class PythonProcess extends ExecutableProcess {
    public PythonProcess(String name, String script, List<String> arguments) {
        super(name, pythonExecutable(), buildArguments(script, arguments));
    }

    public PythonProcess(String name, String script) { this(name, script, List.of()); }
    private static String pythonExecutable() { return System.getenv().getOrDefault("PYTHON", "python"); }
    private static List<String> buildArguments(String script, List<String> arguments) { List<String> values = new ArrayList<>(); values.add(script); if (arguments != null) values.addAll(arguments); return values; }
}
