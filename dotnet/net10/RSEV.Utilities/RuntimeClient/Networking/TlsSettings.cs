using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using RSEV.Utilities.Networking.Security;

namespace RSEV.Utilities.RuntimeClient.Networking
{
    /// <summary>
    /// Client-seitiges TLS-Konfigurationsmodell für RuntimeClient-Verbindungen.
    /// Wird intern in die framework-weite <see cref="TlsOptions"/>-Repräsentation
    /// umgewandelt, um die bestehende, wiederverwendbare TLS-Schicht zu nutzen.
    /// </summary>
    public class TlsSettings
    {
        /// <summary>
        /// Optionales Client-Zertifikat für die gegenseitige Authentifizierung (mTLS).
        /// </summary>
        public X509Certificate2? ClientCertificate { get; set; }

        /// <summary>
        /// Gibt an, ob das Server-Zertifikat validiert werden soll.
        /// Sollte in Produktionsumgebungen immer true sein.
        /// </summary>
        public bool ValidateServerCertificate { get; set; } = true;

        /// <summary>
        /// Die zulässigen TLS-Protokollversionen.
        /// </summary>
        public SslProtocols AllowedProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

        /// <summary>
        /// Konvertiert diese client-seitigen Einstellungen in die framework-weiten
        /// <see cref="TlsOptions"/>, die von der <see cref="Networking.Security.TlsStreamFactory"/>
        /// konsumiert werden.
        /// </summary>
        /// <param name="targetHost">Der Hostname des Zielservers (für SNI/Zertifikatsvalidierung).</param>
        /// <returns>Eine äquivalente TlsOptions-Instanz.</returns>
        public TlsOptions ToTlsOptions(string targetHost)
        {
            return new TlsOptions
            {
                Enabled = true,
                TargetHost = targetHost,
                EnabledProtocols = AllowedProtocols,
                AllowInvalidCertificates = !ValidateServerCertificate
            };
        }
    }
}
