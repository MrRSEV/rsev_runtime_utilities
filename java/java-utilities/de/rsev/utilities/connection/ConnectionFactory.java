package de.rsev.utilities.connection;

import de.rsev.utilities.database.IDatabaseAdapter;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Objects;

public final class ConnectionFactory {
    private final IDatabaseAdapter adapter;
    public ConnectionFactory(IDatabaseAdapter adapter) { this.adapter = Objects.requireNonNull(adapter); }
    public Connection create(ConnectionConfiguration configuration) throws SQLException { return adapter.createConnection(configuration); }
}
