using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.RuntimeClient.Networking;
using RSEV.Utilities.RuntimeClient.Requests;
using RSEV.Utilities.RuntimeClient.Session;

namespace RSEV.Utilities.RuntimeClient.Client
{
    /// <summary>
    /// High-Level-Client für die Kommunikation mit einem RuntimeApiHost.
    /// Orchestriert TCP/TLS-Transport, Request-Erstellung, Session-Bindung
    /// und Response-Interpretation.
    /// </summary>
    public class RuntimeApiClient
    {
        /// <summary>
        /// Der Zielhost des RuntimeApiHost.
        /// </summary>
        private readonly string _host;

        /// <summary>
        /// Der Zielport des RuntimeApiHost.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Gibt an, ob TLS für die Verbindung verwendet werden soll.
        /// </summary>
        private readonly bool _useTls;

        /// <summary>
        /// Optionale TLS-Einstellungen.
        /// </summary>
        private readonly TlsSettings? _tlsSettings;

        /// <summary>
        /// Die aktuell gebundene Session, sofern vorhanden.
        /// </summary>
        private RuntimeSession? _session;

        /// <summary>
        /// Erstellt einen neuen RuntimeApiClient.
        /// </summary>
        /// <param name="host">Der Zielhost.</param>
        /// <param name="port">Der Zielport.</param>
        /// <param name="useTls">Gibt an, ob TLS verwendet werden soll.</param>
        /// <param name="tlsSettings">Optionale TLS-Einstellungen.</param>
        public RuntimeApiClient(string host, int port, bool useTls = false, TlsSettings? tlsSettings = null)
        {
            _host = host;
            _port = port;
            _useTls = useTls;
            _tlsSettings = tlsSettings;
        }

        /// <summary>
        /// Setzt die Session, die automatisch an alle nachfolgenden Requests
        /// angehängt wird.
        /// </summary>
        /// <param name="session">Die zu bindende Session.</param>
        public void SetSession(RuntimeSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Baut einen Request für die angegebene Route und Payload auf, sendet
        /// ihn und liefert die interpretierte Antwort zurück. Erneuert bei
        /// Bedarf automatisch die gebundene Session, bevor der Request gesendet wird.
        /// </summary>
        /// <param name="route">Die Zielroute.</param>
        /// <param name="payload">Die zu übertragende Nutzlast.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Aufrufs.</param>
        /// <returns>Die empfangene RuntimeResponse.</returns>
        public async Task<RuntimeResponse> CallAsync(string route, object payload, CancellationToken cancellationToken = default)
        {
            await EnsureSessionValidAsync();

            var builder = new RuntimeRequestBuilder()
                .WithRoute(route)
                .WithPayload(payload);

            if (_session != null)
            {
                builder.BindSession(_session);
            }

            var request = builder.Build();
            return await SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Sendet einen bereits fertig konstruierten Request und liefert die
        /// interpretierte Antwort zurück.
        /// </summary>
        /// <param name="request">Der zu sendende Request.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Aufrufs.</param>
        /// <returns>Die empfangene RuntimeResponse.</returns>
        public async Task<RuntimeResponse> SendAsync(RuntimeRequest request, CancellationToken cancellationToken = default)
        {
            await EnsureSessionValidAsync();

            var builder = new RuntimeRequestBuilder()
                .WithRoute(request.Route);

            if (request.Payload != null)
            {
                builder.WithPayload(request.Payload);
            }

            foreach (var header in request.Headers)
            {
                builder.WithHeader(header.Key, header.Value);
            }

            if (_session != null)
            {
                builder.BindSession(_session);
            }

            var requestBytes = builder.BuildBytes();

            var transport = new TcpTransport();
            try
            {
                await transport.ConnectAsync(_host, _port, _useTls, _tlsSettings, cancellationToken);
                await transport.SendAsync(requestBytes, cancellationToken);
                var responseBytes = await transport.ReceiveAsync(cancellationToken);

                return ParseResponse(responseBytes);
            }
            finally
            {
                transport.Close();
            }
        }

        /// <summary>
        /// Stellt sicher, dass die gebundene Session gültig ist. Ist die Session
        /// abgelaufen, wird sie automatisch erneuert.
        /// </summary>
        private async Task EnsureSessionValidAsync()
        {
            if (_session != null && _session.IsExpired)
            {
                await _session.RenewAsync();
            }
        }

        /// <summary>
        /// Parsed eine rohe HTTP/1.1-Antwort (Status-Line, Header, Body) in
        /// eine strukturierte RuntimeResponse.
        /// </summary>
        /// <param name="responseBytes">Die empfangenen Rohdaten.</param>
        /// <returns>Die geparste RuntimeResponse.</returns>
        private static RuntimeResponse ParseResponse(byte[] responseBytes)
        {
            const string separator = "\r\n\r\n";
            var rawText = Encoding.ASCII.GetString(responseBytes);
            var separatorIndex = rawText.IndexOf(separator, StringComparison.Ordinal);

            if (separatorIndex < 0)
            {
                return new RuntimeResponse
                {
                    StatusCode = 502,
                    ErrorMessage = "Ungültige oder unvollständige Antwort empfangen."
                };
            }

            var headerText = rawText.Substring(0, separatorIndex);
            var headerByteLength = Encoding.ASCII.GetByteCount(headerText) + separator.Length;

            var lines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var statusLineParts = lines[0].Split(' ', 3);
            var statusCode = statusLineParts.Length >= 2 && int.TryParse(statusLineParts[1], out var parsedStatus)
                ? parsedStatus
                : 0;

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                var sepIndex = lines[i].IndexOf(':');
                if (sepIndex <= 0)
                {
                    continue;
                }

                headers[lines[i].Substring(0, sepIndex).Trim()] = lines[i].Substring(sepIndex + 1).Trim();
            }

            var bodyLength = Math.Max(0, responseBytes.Length - headerByteLength);
            var body = new byte[bodyLength];
            if (bodyLength > 0)
            {
                Buffer.BlockCopy(responseBytes, headerByteLength, body, 0, bodyLength);
            }

            var response = new RuntimeResponse
            {
                StatusCode = statusCode,
                Headers = headers,
                Body = body
            };

            if (response.IsError)
            {
                response.ErrorMessage = response.GetBodyAsText();
            }

            return response;
        }
    }
}
