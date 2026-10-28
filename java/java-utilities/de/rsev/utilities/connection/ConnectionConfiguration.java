package de.rsev.utilities.connection;

public record ConnectionConfiguration(String provider, String url, String username, String password, int minimumPoolSize, int maximumPoolSize) {
    public ConnectionConfiguration {
        if (provider == null || provider.isBlank()) throw new IllegalArgumentException("provider is required");
        if (url == null || url.isBlank()) throw new IllegalArgumentException("url is required");
        if (minimumPoolSize < 0 || maximumPoolSize < 1 || minimumPoolSize > maximumPoolSize) throw new IllegalArgumentException("invalid pool bounds");
    }
    public ConnectionConfiguration(String provider, String url, String username, String password) { this(provider, url, username, password, 1, 10); }
}
