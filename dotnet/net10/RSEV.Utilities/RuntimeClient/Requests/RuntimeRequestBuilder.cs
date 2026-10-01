using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RSEV.Utilities.RuntimeClient.Session;

namespace RSEV.Utilities.RuntimeClient.Requests
{
    /// <summary>
    /// Fluenter Builder zum Erstellen Engine-konformer <see cref="RuntimeRequest"/>-Instanzen.
    /// Serialisiert Payloads als JSON und erzeugt eine HTTP/1.1-kompatible
    /// Byte-Repräsentation, die direkt vom RuntimeApiHost verarbeitet werden kann.
    /// </summary>
    public class RuntimeRequestBuilder
    {
        /// <summary>
        /// Die Route des zu erstellenden Requests.
        /// </summary>
        private string _route = "/";

        /// <summary>
        /// Die Nutzlast des zu erstellenden Requests.
        /// </summary>
        private object? _payload;

        /// <summary>
        /// Die Header des zu erstellenden Requests.
        /// </summary>
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Die gebundene Session, sofern vorhanden.
        /// </summary>
        private RuntimeSession? _session;

        /// <summary>
        /// Setzt die Route des Requests.
        /// </summary>
        /// <param name="route">Die Zielroute.</param>
        /// <returns>Der Builder zur Verkettung weiterer Aufrufe.</returns>
        public RuntimeRequestBuilder WithRoute(string route)
        {
            _route = route;
            return this;
        }

        /// <summary>
        /// Setzt die Nutzlast des Requests. Wird beim Build als JSON serialisiert.
        /// </summary>
        /// <param name="payload">Die zu übertragende Nutzlast.</param>
        /// <returns>Der Builder zur Verkettung weiterer Aufrufe.</returns>
        public RuntimeRequestBuilder WithPayload(object payload)
        {
            _payload = payload;
            return this;
        }

        /// <summary>
        /// Setzt einen zusätzlichen Header für den Request.
        /// </summary>
        /// <param name="key">Der Header-Name.</param>
        /// <param name="value">Der Header-Wert.</param>
        /// <returns>Der Builder zur Verkettung weiterer Aufrufe.</returns>
        public RuntimeRequestBuilder WithHeader(string key, string value)
        {
            _headers[key] = value;
            return this;
        }

        /// <summary>
        /// Bindet eine Session an den Request. Die Session wird beim Build
        /// automatisch über <see cref="RuntimeSession.AttachToRequest"/> angehängt.
        /// </summary>
        /// <param name="session">Die zu bindende Session.</param>
        /// <returns>Der Builder zur Verkettung weiterer Aufrufe.</returns>
        public RuntimeRequestBuilder BindSession(RuntimeSession session)
        {
            _session = session;
            return this;
        }

        /// <summary>
        /// Erstellt die finale <see cref="RuntimeRequest"/>-Instanz aus den
        /// bisher konfigurierten Werten.
        /// </summary>
        /// <returns>Die erstellte RuntimeRequest.</returns>
        public RuntimeRequest Build()
        {
            var request = new RuntimeRequest
            {
                Route = _route,
                Payload = _payload,
                Headers = new Dictionary<string, string>(_headers)
            };

            _session?.AttachToRequest(request);

            return request;
        }

        /// <summary>
        /// Erstellt die finale RuntimeRequest und konvertiert sie in ein
        /// HTTP/1.1-kompatibles Byte-Array für den Versand über TCP, das
        /// direkt vom RuntimeApiHost geparst werden kann.
        /// </summary>
        /// <returns>Die Byte-Repräsentation des Requests.</returns>
        public byte[] BuildBytes()
        {
            var request = Build();

            byte[] bodyBytes = Array.Empty<byte>();
            if (request.Payload != null)
            {
                var json = JsonSerializer.Serialize(request.Payload);
                bodyBytes = Encoding.UTF8.GetBytes(json);
                request.Headers["Content-Type"] = "application/json; charset=utf-8";
            }

            if (!string.IsNullOrEmpty(request.SessionId))
            {
                request.Headers["X-Session-Id"] = request.SessionId;
            }

            request.Headers["Content-Length"] = bodyBytes.Length.ToString();

            var builder = new StringBuilder();
            builder.Append($"POST {request.Route} HTTP/1.1\r\n");

            foreach (var header in request.Headers)
            {
                builder.Append($"{header.Key}: {header.Value}\r\n");
            }

            builder.Append("\r\n");

            var headerBytes = Encoding.ASCII.GetBytes(builder.ToString());
            var result = new byte[headerBytes.Length + bodyBytes.Length];
            Buffer.BlockCopy(headerBytes, 0, result, 0, headerBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, result, headerBytes.Length, bodyBytes.Length);

            return result;
        }
    }
}
