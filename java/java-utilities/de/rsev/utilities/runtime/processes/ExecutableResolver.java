package de.rsev.utilities.runtime.processes;

import java.io.File;

public final class ExecutableResolver {
    private ExecutableResolver() { }

    public static String resolve(String executable) {
        if (executable == null || executable.isBlank()) throw new IllegalArgumentException("executable must not be blank");
        File direct = new File(executable);
        if (direct.isFile() && direct.canExecute()) return direct.getAbsolutePath();
        String path = System.getenv("PATH");
        if (path != null) {
            for (String entry : path.split(File.pathSeparator)) {
                File candidate = new File(entry, executable);
                if (candidate.isFile() && candidate.canExecute()) return candidate.getAbsolutePath();
            }
        }
        return executable;
    }
}
