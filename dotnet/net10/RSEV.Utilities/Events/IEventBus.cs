using RSEV.Utilities.Messaging;
using System;
using System.Collections.Generic;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Definiert einen zentralen Event Bus, der mehrere Event-Registries
    /// verwaltet und als MessageBus integrierbar ist. Der EventBus ermöglicht
    /// flexible Event-Verwaltung auf Framework-Nutzer-Ebene.
    /// </summary>
    public interface IEventBus : IMessageBus
    {
        /// <summary>
        /// Registriert eine neue Event-Registry für einen Event-Typ.
        /// </summary>
        /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente.</typeparam>
        /// <param name="registry">Die zu registrierende Registry.</param>
        void RegisterEventRegistry<TEventArgs>(IEventRegistry<TEventArgs> registry)
            where TEventArgs : EventArgs;

        /// <summary>
        /// Ruft eine registrierte Event-Registry ab.
        /// </summary>
        /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente.</typeparam>
        /// <param name="eventName">Der Name des Events.</param>
        /// <returns>Die Registry oder null, falls nicht vorhanden.</returns>
        IEventRegistry<TEventArgs> GetRegistry<TEventArgs>(string eventName)
            where TEventArgs : EventArgs;

        /// <summary>
        /// Prüft, ob eine Event-Registry existiert.
        /// </summary>
        /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente.</typeparam>
        /// <param name="eventName">Der Name des Events.</param>
        /// <returns>True, wenn die Registry existiert, sonst false.</returns>
        bool HasRegistry<TEventArgs>(string eventName)
            where TEventArgs : EventArgs;

        /// <summary>
        /// Gibt alle registrierten Event-Namen zurück.
        /// </summary>
        /// <returns>Eine Sammlung aller Event-Namen.</returns>
        IEnumerable<string> GetAllEventNames();
    }
}
