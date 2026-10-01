using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace RSEV.Utilities.Networking.Security
{
    /// <summary>
    /// Kapselt die Konfiguration für eine TLS-gesicherte Verbindung. Kann sowohl
    /// für Server- als auch für Client-Szenarien verwendet werden und ist bewusst
    /// unabhängig von konkreten Protokollimplementierungen (z. B. ApiBuilder),
    /// damit sie framework-weit wiederverwendet werden kann.
    /// </summary>
    public class TlsOptions
    {
        /// <summary>
        /// Gibt an, ob TLS für die Verbindung aktiviert werden soll.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Das Server-Zertifikat, das für TLS-Server-Verbindungen verwendet wird.
        /// Erforderlich, wenn <see cref="Enabled"/> true ist und die Verbindung
        /// serverseitig aufgebaut wird.
        /// </summary>
        public X509Certificate2? ServerCertificate { get; set; }

        /// <summary>
        /// Gibt an, ob der Client ein Zertifikat zur Authentifizierung vorlegen muss.
        /// </summary>
        public bool RequireClientCertificate { get; set; } = false;

        /// <summary>
        /// Die zulässigen SSL/TLS-Protokollversionen. Standardmäßig werden moderne,
        /// sichere Protokolle verwendet (TLS 1.2 und TLS 1.3).
        /// </summary>
        public SslProtocols EnabledProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

        /// <summary>
        /// Gibt an, ob Zertifikatsfehler bei Client-Verbindungen ignoriert werden sollen.
        /// Sollte nur in Entwicklungs- oder Testumgebungen verwendet werden.
        /// </summary>
        public bool AllowInvalidCertificates { get; set; } = false;

        /// <summary>
        /// Der Hostname des Servers, der bei Client-Verbindungen für die
        /// Zertifikatsvalidierung verwendet wird.
        /// </summary>
        public string? TargetHost { get; set; }

        /// <summary>
        /// Erstellt eine deaktivierte TlsOptions-Instanz (kein TLS).
        /// </summary>
        public static TlsOptions None => new TlsOptions { Enabled = false };

        /// <summary>
        /// Erstellt eine TlsOptions-Instanz für einen TLS-Server mit dem angegebenen Zertifikat.
        /// </summary>
        /// <param name="certificate">Das Server-Zertifikat.</param>
        /// <returns>Eine neue TlsOptions-Instanz.</returns>
        public static TlsOptions ForServer(X509Certificate2 certificate)
        {
            return new TlsOptions
            {
                Enabled = true,
                ServerCertificate = certificate
            };
        }

        /// <summary>
        /// Erstellt eine TlsOptions-Instanz für einen TLS-Client mit dem angegebenen Zielhost.
        /// </summary>
        /// <param name="targetHost">Der Hostname des Zielservers.</param>
        /// <returns>Eine neue TlsOptions-Instanz.</returns>
        public static TlsOptions ForClient(string targetHost)
        {
            return new TlsOptions
            {
                Enabled = true,
                TargetHost = targetHost
            };
        }
    }
}
