# Betrieb, Sicherheit und bekannte Eigenschaften

## Sicherheitsgrundsätze

### TLS

- In Produktion `TlsOptions.Enabled = true` und ein gültiges
  `ServerCertificate` verwenden.
- `TlsOptions.AllowInvalidCertificates` beziehungsweise
  `TlsSettings.ValidateServerCertificate = false` nicht dauerhaft einsetzen.
- Nur TLS 1.2/1.3 zulassen, sofern die Zielumgebung dies unterstützt.
- Zertifikate und private Schlüssel nicht in das Repository legen.
- `TlsStreamFactory` verwendet im aktuellen Stand
  `X509RevocationMode.NoCheck`; für Umgebungen mit strenger Revocation-
  Prüfung ist eine zusätzliche Prüfung oder eine Erweiterung nötig.

### Plugins und Assemblies

`AssemblyLoader`, `PluginRegistry`, `MessageBusRegistry` und
`GeneratorPluginLoader` laden DLLs und instanziieren Typen. Eine DLL kann dabei
beliebigen Code im Prozess ausführen. Deshalb:

- nur signierte oder anderweitig vertrauenswürdige Plugin-Dateien akzeptieren,
- Plugin-Verzeichnisse nicht aus Benutzereingaben ungeprüft übernehmen,
- Versions- und Kompatibilitätsprüfung vor dem Laden durchführen,
- Plugin-Fehler isolieren und protokollieren.

### Kryptografie und Session

- `HamacProvider` ist HMAC-SHA-256 und benötigt einen geheimen Schlüssel.
- `Sha256Provider` ist ein Hash ohne Geheimnis und ersetzt keine Signatur.
- `KeyManager.Generate` erzeugt kryptografisch zufällige Schlüssel; die
  Mindestgröße beträgt 16 Bytes.
- JWT- und Refresh-Tokenwerte niemals loggen.
- Der aktuelle `SessionManager` besitzt keine serverseitige Revocation-Liste.
  Ein noch nicht abgelaufener Refresh-Token kann lokal nicht widerrufen werden.
- `JwtTokenProvider.Validate` prüft Signatur und `AccessToken.ExpiresAt`.
  Der `exp`-Claim im JWT wird nicht separat gegen die Property abgeglichen.

### Eingaben und Prozesse

- `ShellProcess` führt Befehle über eine Shell aus; keine ungeprüften
  Benutzereingaben direkt einsetzen.
- Argumente der Prozessadapter werden aktuell als zusammengesetzter String an
  `ProcessStartInfo.Arguments` übergeben. Pfade und Argumente mit speziellen
  Zeichen müssen im Zielsystem getestet werden.
- `RuntimeApiHost` besitzt keine Authentifizierung, Autorisierung,
  Größenlimits oder Rate-Limits. Er sollte nicht ungeschützt ins Internet
  gestellt werden.
- Der Host prüft die HTTP-Methode nicht und unterstützt nur einen kleinen
  Parser. Für öffentliche oder komplexe HTTP-APIs ASP.NET Core verwenden.

## Konfiguration: tatsächliches Verhalten

Alle drei Konfigurationsklassen behandeln eine fehlende Datei als No-op:
`Load` und `LoadAsync` werfen in diesem Fall keine Exception.

| Klasse | Lesen | Schreiben | Typverhalten |
|---|---|---|---|
| `ConfConfig` | `key=value`, `#`-Kommentare | `key=value` | Werte werden als Strings gelesen; `Convert.ChangeType` beim `Get<T>` |
| `JsonConfig` | JSON-Objekt | eingerücktes JSON | Werte können nach dem Laden `JsonElement` sein |
| `YamlConfig` | einfache Schlüssel, Einrückung, Listen | flache `key: value`-Ausgabe | kein vollständiger YAML-Parser |
| `BaseConfig` | keine eigene Dateilogik | keine eigene Dateilogik | speichert Werte im Dictionary |

Bei Konvertierungsfehlern liefert `Get<T>` den Defaultwert von `T`, also zum
Beispiel `0`, `false` oder `null`. Das kann einen falsch geschriebenen Wert
verdecken; produktive Anwendungen sollten gelesene Pflichtwerte zusätzlich
validieren.

## Connection Pool

`ConnectionPool` ist thread-safe für seine interne Sammlung, prüft aber beim
`Rent` nicht, ob eine vorhandene Verbindung noch offen oder gesund ist.
`Return` legt die Verbindung bis zur maximalen Größe ab; überschüssige
Verbindungen werden direkt verworfen. `MinimumPoolSize` aus
`ConnectionConfiguration` dient aktuell nur der Validierung und erzeugt keine
Mindestfüllung.

Die zurückgegebene `DbConnection` muss entweder selbst verwaltet oder mit
`ConnectionManager.Return` zurückgegeben werden. Eine Kombination aus
`using` und anschließendem `Return` führt zu einer bereits verworfenen
Verbindung im Pool und sollte vermieden werden.

## Logging

`SystemLog` schreibt nach `Init()` nach `latest.log` und rotiert eine vorhandene
Datei. Unter Windows ist das Standardverzeichnis `./log` unterhalb von
`AppContext.BaseDirectory`; auf Nicht-Windows-Systemen wird
`/var/log/<BaseDirectoryName>` verwendet. Ein explizites Verzeichnis im
Konstruktor hat Vorrang.

Nach `Close()` werden weitere Logaufrufe ignoriert. Die Console-Ausgabe ist
farbig, die Logdatei enthält den formatierten Text ohne Console-Farbe.

## Fehlerbehandlung

- `RuntimeApiHost`: unbekannte Route → 404; Endpoint-Exception → 500.
- `RuntimeApiClient`: unvollständige HTTP-Antwort → 502 und `ErrorMessage`.
- `HttpApiClient`: HTTP-Status ab 500 wird gemäß `RetryPolicy` erneut versucht;
  Exceptions aus `HttpClient` werden nicht in `ApiResponse` umgewandelt.
- `EventRegistry`: Exceptions aus einzelnen Handlern/Listenern werden beim
  Publizieren nicht weitergeworfen.
- `RuntimeSubprocessBase`: Start-/Stop-Exceptions setzen den Zustand auf
  `Faulted` und liefern `false`.
- `UnexpectedExitHandler`: setzt `IsRunning` auf `false` und deaktiviert
  Plugins; Controller, Prozesse und Message-Busse werden nicht automatisch
  vollständig beendet.

## Bekannte API-Besonderheiten des aktuellen Builds

### `SystemMail` liegt in einem verschachtelten Namespace

Die Quelldatei `Communication/SystemMail.cs` deklariert den Namespace doppelt.
Der tatsächlich kompilierte Typ ist daher:

```csharp
RSEV.Utilities.Communication.RSEV.Utilities.Communication.SystemMail
```

Ein Alias kann die Verwendung lesbarer machen:

```csharp
using SystemMail =
    RSEV.Utilities.Communication.RSEV.Utilities.Communication.SystemMail;
```

`MailOptions` liegt dagegen wie erwartet in
`RSEV.Utilities.Communication`.

### API-Host-Modelle sind bewusst minimal

`EndpointRequest` enthält nur `Route`, `Headers` und `Body`. Es gibt kein
`Method`-Property und keine `GetBodyAsText()`-Methode. `EndpointResponse` bietet
aktuell `FromText`, aber kein `FromJson`. JSON muss im Endpoint selbst erzeugt
werden, zum Beispiel mit `JsonSerializer.Serialize` und UTF-8-Bytes.

### `TlsSettings.ClientCertificate`

Die Property existiert im Clientmodell, wird aber in `ToTlsOptions` derzeit
nicht in `TlsOptions` übernommen. Für mTLS darf diese Property daher nicht als
ausreichende Implementierung betrachtet werden.

### Standard-Logger im `BaseRuntimeContext`

Der Konstruktor von `BaseRuntimeContext` erzeugt den
`RuntimeProcessController` und den `RuntimeApiHost` vor der Zuweisung der
`Logger`-Property. Die Standard-Factories erhalten dadurch aktuell keinen
Logger. Wer Host- oder Prozesslogs über den Kontext benötigt, sollte die
Factories beziehungsweise den Kontext explizit anpassen und dieses Verhalten
testen.

