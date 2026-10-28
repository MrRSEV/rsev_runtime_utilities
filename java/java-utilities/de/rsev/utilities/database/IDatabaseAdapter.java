package de.rsev.utilities.database;

import de.rsev.utilities.connection.ConnectionConfiguration;
import java.sql.Connection;
import java.sql.SQLException;

public interface IDatabaseAdapter {
    String provider();
    Connection createConnection(ConnectionConfiguration configuration) throws SQLException;
}
