using System;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Definiert einen Event-Typ mit Metadaten. Ein Event kann von Handlern
    /// verarbeitet oder von Listenern empfangen werden.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public interface IEvent<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Der eindeutige Name des Events.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Eine Beschreibung des Events.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Der Typ der Ereignisargumente.
        /// </summary>
        Type EventArgsType => typeof(TEventArgs);
    }
}
