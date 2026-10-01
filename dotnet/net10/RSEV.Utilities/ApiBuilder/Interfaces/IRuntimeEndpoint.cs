using System.Threading.Tasks;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Definiert einen Endpoint, der von einem RuntimeApiHost bedient werden kann.
    /// </summary>
    public interface IRuntimeEndpoint
    {
        /// <summary>
        /// Die Route, unter der dieser Endpoint erreichbar ist (z. B. "/api/status").
        /// </summary>
        string Route { get; }

        /// <summary>
        /// Verarbeitet eine eingehende Anfrage und liefert eine Antwort zurück.
        /// </summary>
        /// <param name="request">Die eingehende Anfrage.</param>
        /// <returns>Die zu sendende Antwort.</returns>
        Task<EndpointResponse> HandleAsync(EndpointRequest request);
    }
}
