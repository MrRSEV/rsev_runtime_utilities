package de.rsev.utilities.session;

import java.time.Duration;

public interface IAccessTokenProvider {
    AccessToken create(String subject, Duration lifetime);
    boolean validate(AccessToken token);
}
