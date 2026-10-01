package de.rsev.utilities.runtime.processes;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.function.BiConsumer;

/** Base implementation with lifecycle tracking and output events. */
public abstract class RuntimeSubprocessBase implements IRuntimeSubprocess {
    protected volatile Process process;
    private final String processId = UUID.randomUUID().toString().replace("-", "").substring(0, 12);
    private final String name;
    private volatile SubprocessState state = SubprocessState.CREATED;
    private volatile Integer exitCode;
    private final Map<String, Object> properties = new ConcurrentHashMap<>();
    private final CopyOnWriteArrayList<BiConsumer<IRuntimeSubprocess, String>> stdoutHandlers = new CopyOnWriteArrayList<>();
    private final CopyOnWriteArrayList<BiConsumer<IRuntimeSubprocess, String>> stderrHandlers = new CopyOnWriteArrayList<>();

    protected RuntimeSubprocessBase(String name) {
        if (name == null || name.isBlank()) throw new IllegalArgumentException("name must not be blank");
        this.name = name;
    }

    @Override public String getProcessId() { return processId; }
    @Override public String getName() { return name; }
    @Override public SubprocessState getState() { return state; }
    @Override public Integer getExitCode() { return exitCode; }
    @Override public Map<String, Object> getProperties() { return properties; }
    @Override public void addStdOutHandler(BiConsumer<IRuntimeSubprocess, String> handler) { if (handler != null) stdoutHandlers.addIfAbsent(handler); }
    @Override public void addStdErrHandler(BiConsumer<IRuntimeSubprocess, String> handler) { if (handler != null) stderrHandlers.addIfAbsent(handler); }

    @Override public CompletableFuture<Boolean> startAsync() { return CompletableFuture.supplyAsync(this::start); }

    public synchronized boolean start() {
        if (state == SubprocessState.RUNNING && process != null && process.isAlive()) return true;
        state = SubprocessState.STARTING;
        exitCode = null;
        try {
            process = new ProcessBuilder(command()).redirectErrorStream(false).start();
            state = SubprocessState.RUNNING;
            startReader(process, true);
            startReader(process, false);
            CompletableFuture.runAsync(this::watchExit);
            return true;
        } catch (IOException error) {
            state = SubprocessState.FAULTED;
            return false;
        }
    }

    @Override public CompletableFuture<Boolean> stopAsync() { return CompletableFuture.supplyAsync(this::stop); }

    public synchronized boolean stop() {
        if (process == null || !process.isAlive()) {
            state = SubprocessState.STOPPED;
            return true;
        }
        state = SubprocessState.STOPPING;
        process.destroy();
        try {
            if (!process.waitFor(5, java.util.concurrent.TimeUnit.SECONDS)) {
                process.destroyForcibly();
                process.waitFor(5, java.util.concurrent.TimeUnit.SECONDS);
            }
        } catch (InterruptedException error) {
            Thread.currentThread().interrupt();
            return false;
        }
        exitCode = process.exitValue();
        state = SubprocessState.STOPPED;
        return true;
    }

    @Override public CompletableFuture<Boolean> restartAsync() {
        return stopAsync().thenCompose(ignored -> startAsync());
    }

    @Override public CompletableFuture<Boolean> checkHealthAsync() {
        return CompletableFuture.completedFuture(process != null && process.isAlive() && state == SubprocessState.RUNNING);
    }

    protected abstract List<String> command();

    private void startReader(Process runningProcess, boolean stdout) {
        Thread reader = new Thread(() -> {
            try (BufferedReader input = new BufferedReader(new InputStreamReader(
                    stdout ? runningProcess.getInputStream() : runningProcess.getErrorStream(), StandardCharsets.UTF_8))) {
                String line;
                while ((line = input.readLine()) != null) {
                    for (BiConsumer<IRuntimeSubprocess, String> handler : stdout ? stdoutHandlers : stderrHandlers) {
                        try { handler.accept(this, line); } catch (RuntimeException ignored) { }
                    }
                }
            } catch (IOException ignored) { }
        }, name + (stdout ? "-stdout" : "-stderr"));
        reader.setDaemon(true);
        reader.start();
    }

    private void watchExit() {
        try {
            int code = process.waitFor();
            exitCode = code;
            if (state != SubprocessState.STOPPING) state = code == 0 ? SubprocessState.STOPPED : SubprocessState.FAULTED;
        } catch (InterruptedException error) {
            Thread.currentThread().interrupt();
            state = SubprocessState.FAULTED;
        }
    }
}
