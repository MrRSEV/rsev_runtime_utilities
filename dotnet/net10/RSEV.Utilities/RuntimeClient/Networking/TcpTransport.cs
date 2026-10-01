using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Networking.Security;

namespace RSEV.Utilities.RuntimeClient.Networking
{
    /// <summary>
    /// Stellt einen rohen TCP-Transport mit optionalem TLS-Wrapping bereit.
    /// Nutzt intern die framework-weite <see cref="TlsStreamFactory"/>, um
    /// Verbindungen bei Bedarf über TLS abzusichern.
    /// </summary>
    public class TcpTransport
    {
        /// <summary>
        /// Die aktuell aktive Verbindung, sofern eine Verbindung hergestellt wurde.
        /// </summary>
        private TcpConnection? _connection;

        /// <summary>
        /// Baut eine TCP-Verbindung zum angegebenen Host und Port auf. Wird
        /// <paramref name="useTls"/> auf true gesetzt, erfolgt zusätzlich ein
        /// TLS-Handshake über die bestehende Security-Schicht.
        /// </summary>
        /// <param name="host">Der Zielhost.</param>
        /// <param name="port">Der Zielport.</param>
        /// <param name="useTls">Gibt an, ob TLS verwendet werden soll.</param>
        /// <param name="tlsSettings">Optionale TLS-Einstellungen (nur relevant wenn useTls true ist).</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Verbindungsaufbaus.</param>
        /// <returns>Eine aktive TcpConnection.</returns>
        public async Task<TcpConnection> ConnectAsync(
            string host,
            int port,
            bool useTls,
            TlsSettings? tlsSettings = null,
            CancellationToken cancellationToken = default)
        {
            var client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);

            Stream stream;
            if (useTls)
            {
                var settings = tlsSettings ?? new TlsSettings();
                var tlsOptions = settings.ToTlsOptions(host);
                stream = await TlsStreamFactory.CreateClientStreamAsync(client, tlsOptions, cancellationToken);
            }
            else
            {
                stream = client.GetStream();
            }

            _connection = new TcpConnection(client, stream, useTls);
            return _connection;
        }

        /// <summary>
        /// Sendet Rohdaten über die aktive Verbindung.
        /// </summary>
        /// <param name="data">Die zu sendenden Bytes.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Sendevorgangs.</param>
        public async Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            EnsureConnected();
            await _connection!.Stream.WriteAsync(data, cancellationToken);
            await _connection.Stream.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Empfängt verfügbare Rohdaten von der aktiven Verbindung.
        /// </summary>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Empfangsvorgangs.</param>
        /// <returns>Die empfangenen Bytes.</returns>
        public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            EnsureConnected();

            using var buffer = new MemoryStream();
            var chunk = new byte[8192];

            // Liest, solange Daten sofort verfügbar sind (NetworkStream.DataAvailable),
            // damit variable Antwortgrößen unterstützt werden, ohne auf EOF zu warten.
            int read = await _connection!.Stream.ReadAsync(chunk, cancellationToken);
            if (read > 0)
            {
                buffer.Write(chunk, 0, read);
            }

            while (_connection.Client.Available > 0)
            {
                read = await _connection.Stream.ReadAsync(chunk, cancellationToken);
                if (read <= 0)
                {
                    break;
                }
                buffer.Write(chunk, 0, read);
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Schließt die aktive Verbindung.
        /// </summary>
        public void Close()
        {
            _connection?.Close();
            _connection = null;
        }

        /// <summary>
        /// Stellt sicher, dass eine aktive Verbindung besteht.
        /// </summary>
        private void EnsureConnected()
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Es besteht keine aktive Verbindung. ConnectAsync muss zuerst aufgerufen werden.");
            }
        }
    }
}
