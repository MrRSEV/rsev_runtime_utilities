package de.rsev.utilities.connection;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.Objects;

public final class ConnectionManager implements IConnectionManager {
    private final ConnectionFactory factory;
    private final ConnectionPool pool;
    public ConnectionManager(ConnectionFactory factory, ConnectionConfiguration configuration) { this.factory = Objects.requireNonNull(factory); this.pool = new ConnectionPool(configuration.maximumPoolSize()); }
    @Override public Connection open(ConnectionConfiguration configuration) throws SQLException { return pool.rent(() -> { try { return factory.create(configuration); } catch (SQLException exception) { throw new ConnectionFailure(exception); } }); }
    @Override public void giveBack(Connection connection) { pool.giveBack(connection); }
    @Override public void close() { pool.close(); }
    private static final class ConnectionFailure extends RuntimeException { ConnectionFailure(SQLException cause) { super(cause); } }
}
