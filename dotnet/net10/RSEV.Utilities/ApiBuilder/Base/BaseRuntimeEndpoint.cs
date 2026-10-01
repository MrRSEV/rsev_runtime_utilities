using System.Threading.Tasks;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Abstrakte Basisklasse für Runtime-Endpoints. Sie definiert die Route und
    /// stellt eine Hook-Methode OnHandleAsync bereit, die von konkreten Endpoints
    /// überschrieben werden kann.
    /// </summary>
    public abstract class BaseRuntimeEndpoint : IRuntimeEndpoint
    {
        /// <summary>
        /// Die Route, unter der dieser Endpoint erreichbar ist (z. B. "/api/status").
        /// </summary>
        public virtual string Route { get; protected set; }

        /// <summary>
        /// Erstellt einen neuen BaseRuntimeEndpoint mit der angegebenen Route.
        /// </summary>
        /// <param name="route">Die Route des Endpoints.</param>
        protected BaseRuntimeEndpoint(string route)
        {
            Route = route;
        }

        /// <summary>
        /// Verarbeitet eine eingehende Anfrage. Ruft intern OnHandleAsync auf.
        /// </summary>
        /// <param name="request">Die eingehende Anfrage.</param>
        /// <returns>Die zu sendende Antwort.</returns>
        public virtual Task<EndpointResponse> HandleAsync(EndpointRequest request)
        {
            return OnHandleAsync(request);
        }

        /// <summary>
        /// Hook-Methode für die Verarbeitung der Anfrage. Muss von konkreten
        /// Endpoints überschrieben werden.
        /// </summary>
        /// <param name="request">Die eingehende Anfrage.</param>
        /// <returns>Die zu sendende Antwort.</returns>
        protected abstract Task<EndpointResponse> OnHandleAsync(EndpointRequest request);
    }
}
