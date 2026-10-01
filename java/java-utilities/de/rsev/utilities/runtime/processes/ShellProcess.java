package de.rsev.utilities.runtime.processes;

import java.util.List;

public final class ShellProcess extends ExecutableProcess {
    public ShellProcess(String name, String command) {
        super(name,
            System.getProperty("os.name", "").toLowerCase().contains("win") ? "cmd.exe" : "/bin/sh",
            System.getProperty("os.name", "").toLowerCase().contains("win") ? List.of("/c", command) : List.of("-c", command));
    }
}
