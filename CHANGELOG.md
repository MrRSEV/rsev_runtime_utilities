# Changelog

Alle relevanten Änderungen an RSEV Utilities werden in diesem Dokument
versioniert festgehalten.

## [3.0.0] - 2026-10-28

### Plattformübergreifende Module

- Database-Adapter für MySQL, MSSQL/SQL Server, PostgreSQL und SQLite ergänzt.
- Provider-neutrale Connection-Konfiguration, ConnectionFactory,
  ConnectionPool und ConnectionManager ergänzt.
- Crypto-Verträge mit HAMAC/HMAC-SHA256- und SHA-256-Providern ergänzt.
- Key-Management sowie AccessToken-, RefreshToken- und SessionContext-Modelle
  ergänzt.
- JWT-Token-Provider und SessionManager-Verträge ergänzt.
- API-Modelle, HTTP-Clients, RetryPolicy und RateLimitPolicy ergänzt.
- Versionierte Generator-Plugin-Verträge, Registry und Discovery ergänzt.

### Runtime und Architektur

- Runtime-API-Hosts, Endpoint-Controller, Request-Builder und Runtime-Clients
  plattformspezifisch umgesetzt.
- Logging, Events, Messaging, Commands, Dependency Injection und
  Prozess-Lifecycle in den Plattformimplementierungen dokumentiert und
  erweitert.
- Die Implementierungen bleiben auf die jeweiligen Standardbibliotheken bzw.
  Systemklassen der Plattformen beschränkt.
- Das Manifest wurde als gemeinsame Feature- und Architekturdefinition für
  PHP, Java, .NET, Python und Rust dokumentiert.

### Dokumentation

- Plattform-Wikis als DokuWiki-kompatible `.txt`-Seiten angelegt.
- Je Plattform eine strukturierte `README.txt` und `API-Referenz.txt` ergänzt.
- Plattform-README-Dateien um Installation, Architektur, Module, Beispiele,
  Doku-Verweise und Versionsinformationen aktualisiert.
- Root-README um Plattformstatus und Wiki-Verweise erweitert.

## [Unreleased]

- Weitere Paritätsprüfungen zwischen den fünf Plattformimplementierungen.
- Zusätzliche Integrationsbeispiele für Datenbankprovider und API-Endpunkte.
- Erweiterung der automatisierten Test- und Dokumentationsabdeckung.
