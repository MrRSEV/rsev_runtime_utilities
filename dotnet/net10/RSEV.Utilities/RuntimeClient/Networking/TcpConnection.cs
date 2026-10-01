using System.IO;
using System.Net.Sockets;

namespace RSEV.Utilities.RuntimeClient.Networking
{
    /// <summary>
    /// Repräsentiert eine aktive TCP-Verbindung, inklusive des zugrunde liegenden
    /// Streams (ggf. TLS-gesichert) und des zugehörigen TcpClient.
    /// </summary>
    public class TcpConnection
    {
        /// <summary>
        /// Der Stream, über den Daten gelesen und geschrieben werden (NetworkStream oder SslStream).
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Gibt an, ob diese Verbindung TLS-gesichert ist.
        /// </summary>
        public bool IsTls { get; }

        /// <summary>
        /// Der zugrunde liegende TcpClient.
        /// </summary>
        public TcpClient Client { get; }

        /// <summary>
        /// Erstellt eine neue TcpConnection-Instanz.
        /// </summary>
        /// <param name="client">Der zugrunde liegende TcpClient.</param>
        /// <param name="stream">Der Stream für Lese-/Schreiboperationen.</param>
        /// <param name="isTls">Gibt an, ob TLS aktiv ist.</param>
        public TcpConnection(TcpClient client, Stream stream, bool isTls)
        {
            Client = client;
            Stream = stream;
            IsTls = isTls;
        }

        /// <summary>
        /// Schließt den Stream und die zugrunde liegende TCP-Verbindung.
        /// </summary>
        public void Close()
        {
            Stream.Dispose();
            Client.Dispose();
        }
    }
}
