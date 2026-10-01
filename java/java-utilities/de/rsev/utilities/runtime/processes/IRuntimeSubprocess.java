package de.rsev.utilities.runtime.processes;

import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.function.BiConsumer;

public interface IRuntimeSubprocess {
    String getProcessId();
    String getName();
    String getExecutable();
    String[] getArguments();
    SubprocessState getState();
    Integer getExitCode();
    Map<String, Object> getProperties();
    void addStdOutHandler(BiConsumer<IRuntimeSubprocess, String> handler);
    void addStdErrHandler(BiConsumer<IRuntimeSubprocess, String> handler);
    CompletableFuture<Boolean> startAsync();
    CompletableFuture<Boolean> stopAsync();
    CompletableFuture<Boolean> restartAsync();
    CompletableFuture<Boolean> checkHealthAsync();
}
