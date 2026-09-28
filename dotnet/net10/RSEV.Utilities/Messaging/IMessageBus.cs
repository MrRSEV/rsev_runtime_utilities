using RSEV.Utilities.Runtime;
using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Messaging
{
    /// <summary>
    /// Definiert die grundlegende Struktur eines Message Busses. Message Buses
    /// können registriert, konstruiert, initialisiert und heruntergefahren werden.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Der eindeutige Name des Message Busses.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Eine kurze Beschreibung des Message Busses.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Wird aufgerufen, wenn der Message Bus initialisiert werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void Initialize(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Message Bus initialisiert werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task InitializeAsync(IRuntimeContext context);

        /// <summary>
        /// Wird aufgerufen, wenn der Message Bus heruntergefahren werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void Shutdown(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Message Bus heruntergefahren werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task ShutdownAsync(IRuntimeContext context);
    }
}
