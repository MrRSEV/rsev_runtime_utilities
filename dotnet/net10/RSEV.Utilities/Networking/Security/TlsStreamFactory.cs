using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace RSEV.Utilities.Networking.Security
{
    /// <summary>
    /// Stellt wiederverwendbare Factory-Methoden zur Verfügung, um einen rohen
    /// NetworkStream optional in einen TLS-gesicherten SslStream zu verwandeln.
    /// Verwendet ausschließlich die .NET-eigenen Crypto-/TLS-Klassen aus
    /// System.Net.Security und System.Security.Cryptography.
    /// </summary>
    public static class TlsStreamFactory
    {
        /// <summary>
        /// Erstellt einen Stream für eine Server-seitige Verbindung. Ist TLS
        /// aktiviert, wird der NetworkStream in einen authentifizierten SslStream
        /// verpackt, andernfalls wird der rohe NetworkStream zurückgegeben.
        /// </summary>
        /// <param name="client">Der verbundene TcpClient.</param>
        /// <param name="options">Die TLS-Konfiguration.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Handshakes.</param>
        /// <returns>Ein Stream, über den Daten gelesen und geschrieben werden können.</returns>
        public static async Task<Stream> CreateServerStreamAsync(
            TcpClient client,
            TlsOptions options,
            CancellationToken cancellationToken = default)
        {
            var networkStream = client.GetStream();

            if (options == null || !options.Enabled)
            {
                return networkStream;
            }

            if (options.ServerCertificate == null)
            {
                throw new InvalidOperationException(
                    "TLS ist aktiviert, aber kein ServerCertificate wurde in den TlsOptions angegeben.");
            }

            var sslStream = new SslStream(networkStream, leaveInnerStreamOpen: false);

            var serverAuthOptions = new SslServerAuthenticationOptions
            {
                ServerCertificate = options.ServerCertificate,
                ClientCertificateRequired = options.RequireClientCertificate,
                EnabledSslProtocols = options.EnabledProtocols,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck
            };

            await sslStream.AuthenticateAsServerAsync(serverAuthOptions, cancellationToken);

            return sslStream;
        }

        /// <summary>
        /// Erstellt einen Stream für eine Client-seitige Verbindung. Ist TLS
        /// aktiviert, wird der NetworkStream in einen authentifizierten SslStream
        /// verpackt, andernfalls wird der rohe NetworkStream zurückgegeben.
        /// </summary>
        /// <param name="client">Der verbundene TcpClient.</param>
        /// <param name="options">Die TLS-Konfiguration.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Handshakes.</param>
        /// <returns>Ein Stream, über den Daten gelesen und geschrieben werden können.</returns>
        public static async Task<Stream> CreateClientStreamAsync(
            TcpClient client,
            TlsOptions options,
            CancellationToken cancellationToken = default)
        {
            var networkStream = client.GetStream();

            if (options == null || !options.Enabled)
            {
                return networkStream;
            }

            if (string.IsNullOrWhiteSpace(options.TargetHost))
            {
                throw new InvalidOperationException(
                    "TLS ist aktiviert, aber kein TargetHost wurde in den TlsOptions angegeben.");
            }

            var sslStream = new SslStream(
                networkStream,
                leaveInnerStreamOpen: false,
                userCertificateValidationCallback: (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (options.AllowInvalidCertificates)
                    {
                        return true;
                    }

                    return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                });

            var clientAuthOptions = new SslClientAuthenticationOptions
            {
                TargetHost = options.TargetHost,
                EnabledSslProtocols = options.EnabledProtocols,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck
            };

            await sslStream.AuthenticateAsClientAsync(clientAuthOptions, cancellationToken);

            return sslStream;
        }
    }
}
