package de.rsev.utilities.session;

public record SessionContext(String subject, AccessToken accessToken, RefreshToken refreshToken) { }
