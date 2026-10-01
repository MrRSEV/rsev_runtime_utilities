package de.rsev.utilities.runtime.processes;

import java.util.ArrayList;
import java.util.List;

public final class DotNetProcess extends ExecutableProcess {
    public DotNetProcess(String name, String assembly, List<String> arguments) {
        super(name, "dotnet", buildArguments(assembly, arguments));
    }

    public DotNetProcess(String name, String assembly) { this(name, assembly, List.of()); }
    private static List<String> buildArguments(String assembly, List<String> arguments) { List<String> values = new ArrayList<>(); values.add(assembly); if (arguments != null) values.addAll(arguments); return values; }
}
