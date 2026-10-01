using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RSEV.Utilities.RuntimeClient.Requests;

namespace RSEV.Utilities.RuntimeClient.Session
{
    /// <summary>
    /// Repräsentiert eine Session, die an Requests gebunden werden kann, um
    /// Authentifizierungs- oder Zustandsinformationen über mehrere Aufrufe
    /// hinweg zu transportieren.
    /// </summary>
    public class RuntimeSession
    {
        /// <summary>
        /// Die eindeutige ID der Session.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Der Zeitpunkt, zu dem die Session abläuft.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Zusätzliche, beliebige Metadaten der Session.
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gibt an, ob die Session bereits abgelaufen ist.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        /// <summary>
        /// Erstellt eine neue RuntimeSession.
        /// </summary>
        /// <param name="sessionId">Die eindeutige ID der Session.</param>
        /// <param name="expiresAt">Der Ablaufzeitpunkt der Session.</param>
        public RuntimeSession(string sessionId, DateTime expiresAt)
        {
            SessionId = sessionId;
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Bindet diese Session an den angegebenen Request, indem die SessionId
        /// gesetzt wird.
        /// </summary>
        /// <param name="request">Der Request, an den die Session gebunden werden soll.</param>
        public virtual void AttachToRequest(RuntimeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.SessionId = SessionId;
        }

        /// <summary>
        /// Erneuert die Session. Diese Methode ist ein Erweiterungspunkt: die
        /// Standardimplementierung verlängert lediglich die lokale Ablaufzeit
        /// um eine Stunde. Abgeleitete Klassen können diese Methode
        /// überschreiben, um die Erneuerung gegen einen entfernten Server
        /// durchzuführen (z. B. über einen dedizierten Renew-Endpoint).
        /// </summary>
        public virtual Task RenewAsync()
        {
            ExpiresAt = DateTime.UtcNow.AddHours(1);
            return Task.CompletedTask;
        }
    }
}
