# RSEV.Utilities  
Ein universelles, erweiterbares und zukunftssicheres .NET-Framework für Plugins, Controller, Module und Systemdienste.

RSEV.Utilities ist ein modular aufgebautes Utility-Framework, das Entwicklern sofort eine stabile Grundlage bietet, um komplexe Anwendungen, Services oder Plugin-Systeme zu erstellen.  
Es kombiniert Klarheit, Erweiterbarkeit und architektonische Reinheit – mit dem Ziel, Barrieren zu entfernen und produktive Entwicklung zu ermöglichen.

---

## ? Features

### ?? Universeller RuntimeContext
- Globale Konfiguration (`IConfig`)
- AssemblyLoader für dynamisches Laden von Plugins
- TypeDiscovery für automatische Typ-Erkennung
- PluginRegistry für Aktivierung/Deaktivierung von Plugins
- MessageBusRegistry für Registrierung und Konstruktion benutzerdefinierter Message Buses
- GlobalState für systemweite Zustände
- Integrierter Logger (`ILogger`)

### ?? Lifecycle-System
- `IOnStart` / `IOnStartAsync`
- `IOnStop` / `IOnStopAsync`
- `IFirstRun` für einmalige Initialisierungen
- Saubere Trennung von Start-, Stop- und Setup-Logik

### ?? Logging-System
- `SystemLog` als erweiterbarer Standard-Logger
- Logrotation
- Farbige Konsolenausgabe
- LogLevel: Debug, Info, Warnung, Error, Kritisch
- Austauschbar über `ILogger`

### ??? Stabiler Shutdown & Fehlerbehandlung
- `UnexpectedExitHandler` für kontrollierten Shutdown bei Fehlern
- `ShutdownSummary` für Diagnose und Logging
- Empfehlung zur Nutzung eigener `IOnStop`-Implementierungen

### ?? Plugin- & Modularchitektur
- Dynamisches Laden externer Assemblies
- Automatische Discovery von Controllern, Plugins und Services
- Erweiterbar ohne Änderungen am Kernsystem

### ?? Event- & Messaging-Templates
- `IEventHandler<TEventArgs>` / `BaseEventHandler<TEventArgs>` für Ereignisverarbeitung
- `IEventListener<TEventArgs>` / `BaseEventListener<TEventArgs>` für Ereignisempfang
- `IMessageBus` / `BaseMessageBus` für benutzerdefinierte Message-Bus-Implementierungen
- `MessageBusRegistry` zum Registrieren, Laden und Verwalten von Message Buses

---

## ?? Installation

Einfach das Projekt als NuGet-Paket einbinden (optional, sobald veröffentlicht):

```bash
dotnet add package RSEV.Utilities