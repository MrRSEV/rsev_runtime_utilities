package de.rsev.utilities.session;

public interface ISessionManager {
    SessionContext create(String subject);
    boolean isValid(SessionContext context);
}
