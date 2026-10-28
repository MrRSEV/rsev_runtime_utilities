package de.rsev.utilities.connection;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.function.Supplier;

public final class ConnectionPool implements AutoCloseable {
    private final Queue<Connection> connections = new ConcurrentLinkedQueue<>();
    private final int maximumSize;
    public ConnectionPool(int maximumSize) { if (maximumSize < 1) throw new IllegalArgumentException("maximumSize must be positive"); this.maximumSize = maximumSize; }
    public Connection rent(Supplier<Connection> factory) { Connection value = connections.poll(); return value != null ? value : factory.get(); }
    public boolean giveBack(Connection connection) { if (connections.size() >= maximumSize) { closeQuietly(connection); return false; } return connections.offer(connection); }
    public void clear() { Connection value; while ((value = connections.poll()) != null) closeQuietly(value); }
    @Override public void close() { clear(); }
    private static void closeQuietly(Connection connection) { try { connection.close(); } catch (SQLException ignored) { } }
}
