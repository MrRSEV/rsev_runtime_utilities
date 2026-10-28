package de.rsev.utilities.connection;

import java.sql.Connection;
import java.sql.SQLException;

public interface IConnectionManager extends AutoCloseable {
    Connection open(ConnectionConfiguration configuration) throws SQLException;
    void giveBack(Connection connection);
    @Override void close();
}
