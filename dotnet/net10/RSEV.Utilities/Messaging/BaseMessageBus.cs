using RSEV.Utilities.Runtime;
using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Messaging
{
    /// <summary>
    /// Eine universelle, erweiterbare Basisklasse für Message Buses.
    /// Sie stellt Standardimplementierungen für Initialisierung und Shutdown bereit
    /// und dient als Grundlage für benutzerdefinierte Message Busse.
    /// </summary>
    public abstract class BaseMessageBus : IMessageBus
    {
        /// <summary>
        /// Der eindeutige Name des Message Busses.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Eine kurze Beschreibung des Message Busses.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Erstellt eine neue Instanz des BaseMessageBus.
        /// </summary>
        /// <param name="name">Der eindeutige Name des Message Busses.</param>
        /// <param name="description">Die Beschreibung des Message Busses.</param>
        protected BaseMessageBus(string name, string description = null)
        {
            Name = name;
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Initialisiert den Message Bus.
        /// </summary>
        public virtual void Initialize(IRuntimeContext context)
        {
            OnInitialize(context);
        }

        /// <summary>
        /// Initialisiert den Message Bus asynchron.
        /// </summary>
        public virtual Task InitializeAsync(IRuntimeContext context)
        {
            return OnInitializeAsync(context);
        }

        /// <summary>
        /// Fährt den Message Bus herunter.
        /// </summary>
        public virtual void Shutdown(IRuntimeContext context)
        {
            OnShutdown(context);
        }

        /// <summary>
        /// Fährt den Message Bus asynchron herunter.
        /// </summary>
        public virtual Task ShutdownAsync(IRuntimeContext context)
        {
            return OnShutdownAsync(context);
        }

        /// <summary>
        /// Hook-Methode für die Initialisierung. Kann überschrieben werden.
        /// </summary>
        protected virtual void OnInitialize(IRuntimeContext context)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Asynchroner Hook für die Initialisierung. Kann überschrieben werden.
        /// </summary>
        protected virtual Task OnInitializeAsync(IRuntimeContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook-Methode für das Herunterfahren. Kann überschrieben werden.
        /// </summary>
        protected virtual void OnShutdown(IRuntimeContext context)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Asynchroner Hook für das Herunterfahren. Kann überschrieben werden.
        /// </summary>
        protected virtual Task OnShutdownAsync(IRuntimeContext context)
        {
            return Task.CompletedTask;
        }
    }
}
