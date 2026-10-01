using System;
using System.Collections.Generic;
using System.Linq;

namespace RSEV.Utilities.ApiBuilder
{
    /// <summary>
    /// Verwaltet eine zentrale Sammlung von Runtime-Endpoints. Bietet Registrierung
    /// und Auflösung nach Route.
    /// </summary>
    public class RuntimeEndpointController
    {
        /// <summary>
        /// Der Speicher für registrierte Endpoints, indiziert nach Route.
        /// </summary>
        private readonly Dictionary<string, IRuntimeEndpoint> _endpoints;

        /// <summary>
        /// Lock für Thread-Sicherheit.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Erstellt einen neuen RuntimeEndpointController.
        /// </summary>
        public RuntimeEndpointController()
        {
            _endpoints = new Dictionary<string, IRuntimeEndpoint>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registriert einen neuen Endpoint.
        /// </summary>
        /// <param name="endpoint">Der zu registrierende Endpoint.</param>
        /// <returns>True wenn erfolgreich registriert, false wenn die Route bereits belegt ist.</returns>
        public bool Register(IRuntimeEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            lock (_lock)
            {
                if (_endpoints.ContainsKey(endpoint.Route))
                {
                    return false;
                }

                _endpoints[endpoint.Route] = endpoint;
                return true;
            }
        }

        /// <summary>
        /// Registriert mehrere Endpoints.
        /// </summary>
        /// <param name="endpoints">Die zu registrierenden Endpoints.</param>
        /// <returns>Anzahl der erfolgreich registrierten Endpoints.</returns>
        public int RegisterRange(IEnumerable<IRuntimeEndpoint> endpoints)
        {
            int count = 0;
            foreach (var endpoint in endpoints)
            {
                if (Register(endpoint))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Entfernt einen Endpoint anhand seiner Route.
        /// </summary>
        /// <param name="route">Die Route des zu entfernenden Endpoints.</param>
        /// <returns>True wenn erfolgreich entfernt, false wenn nicht gefunden.</returns>
        public bool Unregister(string route)
        {
            lock (_lock)
            {
                return _endpoints.Remove(route);
            }
        }

        /// <summary>
        /// Prüft, ob ein Endpoint mit der gegebenen Route existiert.
        /// </summary>
        public bool Exists(string route)
        {
            lock (_lock)
            {
                return _endpoints.ContainsKey(route);
            }
        }

        /// <summary>
        /// Löst einen Endpoint anhand seiner Route auf.
        /// </summary>
        /// <param name="route">Die Route des gesuchten Endpoints.</param>
        /// <returns>Der Endpoint, oder null wenn nicht gefunden.</returns>
        public IRuntimeEndpoint? Resolve(string route)
        {
            lock (_lock)
            {
                _endpoints.TryGetValue(route, out var endpoint);
                return endpoint;
            }
        }

        /// <summary>
        /// Ruft alle registrierten Endpoints auf.
        /// </summary>
        public IEnumerable<IRuntimeEndpoint> GetAll()
        {
            lock (_lock)
            {
                return _endpoints.Values.ToList();
            }
        }
    }
}
