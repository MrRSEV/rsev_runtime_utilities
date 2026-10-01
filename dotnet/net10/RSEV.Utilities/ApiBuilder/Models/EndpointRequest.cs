using System.Collections.Generic;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Repräsentiert eine eingehende Anfrage an einen Runtime-Endpoint.
    /// </summary>
    public class EndpointRequest
    {
        /// <summary>
        /// Die angeforderte Route (z. B. "/api/status").
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// Die HTTP-Header der Anfrage.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Der Body der Anfrage als Rohdaten.
        /// </summary>
        public byte[] Body { get; set; } = System.Array.Empty<byte>();
    }
}
