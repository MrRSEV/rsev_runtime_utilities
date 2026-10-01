package de.rsev.utilities.runtime.processes;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ConcurrentHashMap;

/** Thread-safe registry and lifecycle controller for runtime subprocesses. */
public final class RuntimeProcessController {
    private final Map<String, IRuntimeSubprocess> processes = new ConcurrentHashMap<>();

    public boolean register(IRuntimeSubprocess process) {
        if (process == null) throw new IllegalArgumentException("process must not be null");
        return processes.putIfAbsent(process.getProcessId(), process) == null;
    }

    public int registerRange(Iterable<? extends IRuntimeSubprocess> values) {
        int count = 0;
        for (IRuntimeSubprocess process : values) if (register(process)) count++;
        return count;
    }

    public boolean exists(String processId) { return processes.containsKey(processId); }
    public IRuntimeSubprocess get(String processId) { return processes.get(processId); }
    public List<IRuntimeSubprocess> getAll() { return List.copyOf(processes.values()); }
    public List<IRuntimeSubprocess> findByName(String name) { return processes.values().stream().filter(p -> p.getName().equalsIgnoreCase(name)).toList(); }
    public List<IRuntimeSubprocess> findByState(SubprocessState state) { return processes.values().stream().filter(p -> p.getState() == state).toList(); }
    public boolean unregister(String processId) { return processes.remove(processId) != null; }

    public CompletableFuture<Boolean> startAsync(String processId) { IRuntimeSubprocess p = get(processId); return p == null ? CompletableFuture.completedFuture(false) : p.startAsync(); }
    public CompletableFuture<Boolean> stopAsync(String processId) { IRuntimeSubprocess p = get(processId); return p == null ? CompletableFuture.completedFuture(false) : p.stopAsync(); }
    public CompletableFuture<Boolean> startByNameAsync(String name) { List<IRuntimeSubprocess> ps = findByName(name); return ps.isEmpty() ? CompletableFuture.completedFuture(false) : ps.get(0).startAsync(); }
    public CompletableFuture<Boolean> stopByNameAsync(String name) { List<IRuntimeSubprocess> ps = findByName(name); return ps.isEmpty() ? CompletableFuture.completedFuture(false) : ps.get(0).stopAsync(); }

    public CompletableFuture<Integer> startAllAsync() { return runAll(true); }
    public CompletableFuture<Integer> stopAllAsync() { return runAll(false); }

    public CompletableFuture<Map<String, Boolean>> checkAllHealthAsync() {
        List<IRuntimeSubprocess> registered = getAll();
        List<CompletableFuture<Boolean>> futures = registered.stream().map(IRuntimeSubprocess::checkHealthAsync).toList();
        return CompletableFuture.allOf(futures.toArray(CompletableFuture[]::new)).thenApply(ignored -> {
            Map<String, Boolean> result = new ConcurrentHashMap<>();
            for (int index = 0; index < registered.size(); index++) result.put(registered.get(index).getProcessId(), futures.get(index).join());
            return result;
        });
    }

    private CompletableFuture<Integer> runAll(boolean start) {
        List<CompletableFuture<Boolean>> futures = new ArrayList<>();
        for (IRuntimeSubprocess process : getAll()) futures.add(start ? process.startAsync() : process.stopAsync());
        return CompletableFuture.allOf(futures.toArray(CompletableFuture[]::new)).thenApply(ignored -> (int) futures.stream().filter(CompletableFuture::join).count());
    }
}
