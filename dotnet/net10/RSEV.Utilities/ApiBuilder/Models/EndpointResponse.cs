using System.Collections.Generic;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Repräsentiert die Antwort eines Runtime-Endpoints.
    /// </summary>
    public class EndpointResponse
    {
        /// <summary>
        /// Der HTTP-Statuscode der Antwort (z. B. 200, 404, 500).
        /// </summary>
        public int StatusCode { get; set; } = 200;

        /// <summary>
        /// Die HTTP-Header der Antwort.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Der Body der Antwort als Rohdaten.
        /// </summary>
        public byte[] Body { get; set; } = System.Array.Empty<byte>();

        /// <summary>
        /// Erstellt eine einfache Textantwort.
        /// </summary>
        /// <param name="statusCode">Der HTTP-Statuscode.</param>
        /// <param name="text">Der Text-Inhalt der Antwort.</param>
        /// <param name="contentType">Der Content-Type Header (Standard: text/plain).</param>
        /// <returns>Eine neue EndpointResponse-Instanz.</returns>
        public static EndpointResponse FromText(int statusCode, string text, string contentType = "text/plain; charset=utf-8")
        {
            var response = new EndpointResponse
            {
                StatusCode = statusCode,
                Body = System.Text.Encoding.UTF8.GetBytes(text)
            };
            response.Headers["Content-Type"] = contentType;
            return response;
        }
    }
}
