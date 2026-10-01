using System.Collections.Generic;

namespace RSEV.Utilities.RuntimeClient.Requests
{
    /// <summary>
    /// Standardisierte Engine-Response, die vom RuntimeApiClient nach dem
    /// Empfang und Parsen der Rohantwort erzeugt wird.
    /// </summary>
    public class RuntimeResponse
    {
        /// <summary>
        /// Der HTTP-Statuscode der Antwort.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Der Rohinhalt der Antwort.
        /// </summary>
        public byte[] Body { get; set; } = System.Array.Empty<byte>();

        /// <summary>
        /// Die Header der Antwort.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gibt an, ob die Antwort einen Fehler repräsentiert (StatusCode >= 400).
        /// </summary>
        public bool IsError => StatusCode >= 400;

        /// <summary>
        /// Eine optionale Fehlermeldung, gesetzt wenn <see cref="IsError"/> true ist.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gibt den Body als UTF-8-codierten Text zurück.
        /// </summary>
        /// <returns>Der Body als Text.</returns>
        public string GetBodyAsText()
        {
            return System.Text.Encoding.UTF8.GetString(Body);
        }
    }
}
