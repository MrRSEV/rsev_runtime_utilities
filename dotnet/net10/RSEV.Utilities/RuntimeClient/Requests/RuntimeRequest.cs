using System.Collections.Generic;

namespace RSEV.Utilities.RuntimeClient.Requests
{
    /// <summary>
    /// Repräsentiert einen Engine-Request bestehend aus Route, Payload, Headern
    /// und optionaler Session-Bindung.
    /// </summary>
    public class RuntimeRequest
    {
        /// <summary>
        /// Die Zielroute des Requests (z. B. "/api/status").
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// Die Nutzlast des Requests. Wird beim Erzeugen der Byte-Repräsentation
        /// als JSON serialisiert.
        /// </summary>
        public object? Payload { get; set; }

        /// <summary>
        /// Die HTTP-ähnlichen Header des Requests.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Die ID der gebundenen Session, sofern vorhanden.
        /// </summary>
        public string? SessionId { get; set; }
    }
}
