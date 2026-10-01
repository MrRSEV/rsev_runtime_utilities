using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Definiert ein generisches Event-Registry. Eine Registry registriert
    /// und verwaltet Handler sowie Listener für einen bestimmten Event-Typ.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public interface IEventRegistry<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Der Name des Events.
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Registriert einen Handler für diesen Event-Typ.
        /// </summary>
        /// <param name="handler">Der zu registrierende Handler.</param>
        void Subscribe(IEventHandler<TEventArgs> handler);

        /// <summary>
        /// Entfernt einen Handler.
        /// </summary>
        /// <param name="handler">Der zu entfernende Handler.</param>
        void Unsubscribe(IEventHandler<TEventArgs> handler);

        /// <summary>
        /// Registriert einen Listener für diesen Event-Typ.
        /// </summary>
        /// <param name="listener">Der zu registrierende Listener.</param>
        void Subscribe(IEventListener<TEventArgs> listener);

        /// <summary>
        /// Entfernt einen Listener.
        /// </summary>
        /// <param name="listener">Der zu entfernende Listener.</param>
        void Unsubscribe(IEventListener<TEventArgs> listener);

        /// <summary>
        /// Gibt alle registrierten Handler zurück.
        /// </summary>
        /// <returns>Eine Sammlung aller Handler.</returns>
        IReadOnlyCollection<IEventHandler<TEventArgs>> GetHandlers();

        /// <summary>
        /// Gibt alle registrierten Listener zurück.
        /// </summary>
        /// <returns>Eine Sammlung aller Listener.</returns>
        IReadOnlyCollection<IEventListener<TEventArgs>> GetListeners();

        /// <summary>
        /// Publiziert ein Event synchron an alle Handler und Listener.
        /// </summary>
        /// <param name="sender">Der Absender des Events.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        void Publish(object sender, TEventArgs args);

        /// <summary>
        /// Publiziert ein Event asynchron an alle Handler und Listener.
        /// </summary>
        /// <param name="sender">Der Absender des Events.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        Task PublishAsync(object sender, TEventArgs args);
    }
}
