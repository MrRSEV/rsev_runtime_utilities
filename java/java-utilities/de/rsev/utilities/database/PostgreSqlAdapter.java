package de.rsev.utilities.database;

import de.rsev.utilities.connection.ConnectionConfiguration;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public final class PostgreSqlAdapter implements IDatabaseAdapter {
    @Override public String provider() { return "postgresql"; }
    @Override public Connection createConnection(ConnectionConfiguration configuration) throws SQLException { return DriverManager.getConnection(configuration.url(), configuration.username(), configuration.password()); }
}
