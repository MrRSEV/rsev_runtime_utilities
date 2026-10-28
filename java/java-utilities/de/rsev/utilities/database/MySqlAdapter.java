package de.rsev.utilities.database;

import de.rsev.utilities.connection.ConnectionConfiguration;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public final class MySqlAdapter implements IDatabaseAdapter {
    @Override public String provider() { return "mysql"; }
    @Override public Connection createConnection(ConnectionConfiguration configuration) throws SQLException { return DriverManager.getConnection(configuration.url(), configuration.username(), configuration.password()); }
}
