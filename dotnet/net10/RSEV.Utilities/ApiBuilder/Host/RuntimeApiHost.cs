using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Networking.Security;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Ein leichtgewichtiger, framework-eigener API-Host. Lauscht auf einem
    /// TCP-Port, parsed eingehende Anfragen als einfaches HTTP/1.1-Subset
    /// (Request-Line, Header, optionaler Body via Content-Length), löst den
    /// passenden Endpoint über einen internen RuntimeEndpointController auf
    /// und sendet die Antwort zurück. Unterstützt optional TLS über die
    /// wiederverwendbare Networking.Security-Schicht.
    /// </summary>
    public class RuntimeApiHost
    {
        /// <summary>
        /// Der interne Endpoint-Controller, der alle registrierten Endpoints verwaltet.
        /// </summary>
        private readonly RuntimeEndpointController _endpointController;

        /// <summary>
        /// Die TLS-Konfiguration für diesen Host.
        /// </summary>
        private readonly TlsOptions _tlsOptions;

        /// <summary>
        /// Optionaler Logger für Host-Ereignisse.
        /// </summary>
        private readonly ILogger? _logger;

        /// <summary>
        /// Der zugrunde liegende TCP-Listener.
        /// </summary>
        private TcpListener? _listener;

        /// <summary>
        /// Gibt an, ob der Host aktuell läuft.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Erstellt einen neuen RuntimeApiHost.
        /// </summary>
        /// <param name="tlsOptions">Optionale TLS-Konfiguration. Standard: kein TLS.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public RuntimeApiHost(TlsOptions? tlsOptions = null, ILogger? logger = null)
        {
            _endpointController = new RuntimeEndpointController();
            _tlsOptions = tlsOptions ?? TlsOptions.None;
            _logger = logger;
        }

        /// <summary>
        /// Registriert einen Endpoint am internen Controller.
        /// </summary>
        /// <param name="endpoint">Der zu registrierende Endpoint.</param>
        /// <returns>True wenn erfolgreich registriert, false wenn die Route bereits belegt ist.</returns>
        public bool Register(IRuntimeEndpoint endpoint)
        {
            return _endpointController.Register(endpoint);
        }

        /// <summary>
        /// Startet den API-Host auf dem angegebenen Port und nimmt Verbindungen
        /// entgegen, bis das CancellationToken ausgelöst wird.
        /// </summary>
        /// <param name="port">Der Port, auf dem gelauscht werden soll.</param>
        /// <param name="cancellationToken">Ein Token zum Abbrechen des Hosts.</param>
        public async Task StartAsync(int port, CancellationToken cancellationToken = default)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsRunning = true;

            _logger?.LogInfo($"[RuntimeApiHost] Gestartet auf Port {port} (TLS: {_tlsOptions.Enabled}).");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client;
                    try
                    {
                        client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    _ = HandleClientAsync(client, cancellationToken);
                }
            }
            finally
            {
                IsRunning = false;
                _listener.Stop();
                _logger?.LogInfo("[RuntimeApiHost] Gestoppt.");
            }
        }

        /// <summary>
        /// Stoppt den API-Host.
        /// </summary>
        public void Stop()
        {
            _listener?.Stop();
            IsRunning = false;
        }

        /// <summary>
        /// Verarbeitet eine einzelne Client-Verbindung: Stream aufbauen (ggf. TLS),
        /// Request parsen, Endpoint auflösen, Antwort senden.
        /// </summary>
        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                Stream stream;
                try
                {
                    stream = await TlsStreamFactory.CreateServerStreamAsync(client, _tlsOptions, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"[RuntimeApiHost] TLS-Handshake fehlgeschlagen: {ex.Message}");
                    return;
                }

                using (stream)
                {
                    try
                    {
                        var request = await ParseRequestAsync(stream, cancellationToken);
                        if (request == null)
                        {
                            return;
                        }

                        EndpointResponse response;
                        var endpoint = _endpointController.Resolve(request.Route);

                        if (endpoint == null)
                        {
                            response = EndpointResponse.FromText(404, "Not Found");
                        }
                        else
                        {
                            try
                            {
                                response = await endpoint.HandleAsync(request);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError($"[RuntimeApiHost] Endpoint '{request.Route}' hat eine Exception ausgelöst: {ex.Message}");
                                response = EndpointResponse.FromText(500, "Internal Server Error");
                            }
                        }

                        await WriteResponseAsync(stream, response, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"[RuntimeApiHost] Fehler bei der Verarbeitung der Anfrage: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Parsed eine eingehende HTTP/1.1-Anfrage (Request-Line, Header, optionaler
        /// Body via Content-Length) aus dem Stream.
        /// </summary>
        private static async Task<EndpointRequest?> ParseRequestAsync(Stream stream, CancellationToken cancellationToken)
        {
            var headerBytes = await ReadHeaderBlockAsync(stream, cancellationToken);
            if (headerBytes == null)
            {
                return null;
            }

            var headerText = Encoding.ASCII.GetString(headerBytes);
            var lines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (lines.Length == 0 || string.IsNullOrWhiteSpace(lines[0]))
            {
                return null;
            }

            // Request-Line: "METHOD /route HTTP/1.1" - Methode wird bewusst nicht unterschieden.
            var requestLineParts = lines[0].Split(' ');
            var route = requestLineParts.Length >= 2 ? requestLineParts[1] : "/";

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    continue;
                }

                var separatorIndex = lines[i].IndexOf(':');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = lines[i].Substring(0, separatorIndex).Trim();
                var value = lines[i].Substring(separatorIndex + 1).Trim();
                headers[key] = value;
            }

            byte[] body = Array.Empty<byte>();
            if (headers.TryGetValue("Content-Length", out var contentLengthValue) &&
                int.TryParse(contentLengthValue, out var contentLength) &&
                contentLength > 0)
            {
                body = new byte[contentLength];
                var totalRead = 0;
                while (totalRead < contentLength)
                {
                    var read = await stream.ReadAsync(body.AsMemory(totalRead, contentLength - totalRead), cancellationToken);
                    if (read == 0)
                    {
                        break;
                    }
                    totalRead += read;
                }
            }

            return new EndpointRequest
            {
                Route = route,
                Headers = headers,
                Body = body
            };
        }

        /// <summary>
        /// Liest den Header-Block (bis zur leeren Zeile "\r\n\r\n") aus dem Stream.
        /// </summary>
        private static async Task<byte[]?> ReadHeaderBlockAsync(Stream stream, CancellationToken cancellationToken)
        {
            using var buffer = new MemoryStream();
            var singleByte = new byte[1];
            var terminatorMatch = 0;
            var terminator = new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };

            while (true)
            {
                var read = await stream.ReadAsync(singleByte.AsMemory(0, 1), cancellationToken);
                if (read == 0)
                {
                    return buffer.Length > 0 ? buffer.ToArray() : null;
                }

                buffer.WriteByte(singleByte[0]);

                if (singleByte[0] == terminator[terminatorMatch])
                {
                    terminatorMatch++;
                    if (terminatorMatch == terminator.Length)
                    {
                        break;
                    }
                }
                else
                {
                    terminatorMatch = singleByte[0] == terminator[0] ? 1 : 0;
                }
            }

            return buffer.ToArray();
        }

        /// <summary>
        /// Schreibt eine EndpointResponse als HTTP/1.1-Antwort in den Stream.
        /// </summary>
        private static async Task WriteResponseAsync(Stream stream, EndpointResponse response, CancellationToken cancellationToken)
        {
            var statusText = GetStatusText(response.StatusCode);
            var builder = new StringBuilder();
            builder.Append($"HTTP/1.1 {response.StatusCode} {statusText}\r\n");

            if (!response.Headers.ContainsKey("Content-Length"))
            {
                response.Headers["Content-Length"] = response.Body.Length.ToString();
            }

            foreach (var header in response.Headers)
            {
                builder.Append($"{header.Key}: {header.Value}\r\n");
            }

            builder.Append("\r\n");

            var headerBytes = Encoding.ASCII.GetBytes(builder.ToString());
            await stream.WriteAsync(headerBytes, cancellationToken);

            if (response.Body.Length > 0)
            {
                await stream.WriteAsync(response.Body, cancellationToken);
            }

            await stream.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Liefert einen einfachen Status-Text für den gegebenen HTTP-Statuscode.
        /// </summary>
        private static string GetStatusText(int statusCode)
        {
            return statusCode switch
            {
                200 => "OK",
                201 => "Created",
                204 => "No Content",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                405 => "Method Not Allowed",
                500 => "Internal Server Error",
                _ => "Unknown"
            };
        }
    }
}
