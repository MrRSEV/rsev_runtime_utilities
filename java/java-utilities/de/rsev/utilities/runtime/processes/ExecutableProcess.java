package de.rsev.utilities.runtime.processes;

import java.util.ArrayList;
import java.util.List;

public class ExecutableProcess extends RuntimeSubprocessBase {
    private final String executable;
    private final List<String> arguments;

    public ExecutableProcess(String name, String executable, List<String> arguments) {
        super(name);
        this.executable = executable;
        this.arguments = new ArrayList<>(arguments == null ? List.of() : arguments);
    }

    public ExecutableProcess(String name, String executable) { this(name, executable, List.of()); }
    @Override public String getExecutable() { return executable; }
    @Override public String[] getArguments() { return arguments.toArray(String[]::new); }
    @Override protected List<String> command() { List<String> command = new ArrayList<>(); command.add(executable); command.addAll(arguments); return command; }
}
