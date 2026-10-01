using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Eine konkrete Implementierung eines Event-Registrys. Sie verwaltet
    /// Handler und Listener für einen bestimmten Event-Typ und ermöglicht
    /// das synchrone und asynchrone Publizieren von Events.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public class EventRegistry<TEventArgs> : IEventRegistry<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Der Name des Events.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Die Liste der registrierten Handler.
        /// </summary>
        protected readonly List<IEventHandler<TEventArgs>> _handlers = new();

        /// <summary>
        /// Die Liste der registrierten Listener.
        /// </summary>
        protected readonly List<IEventListener<TEventArgs>> _listeners = new();

        /// <summary>
        /// Ein Lock-Objekt für Thread-Safety.
        /// </summary>
        protected readonly object _lockObject = new();

        /// <summary>
        /// Erstellt eine neue Instanz eines EventRegistry.
        /// </summary>
        /// <param name="eventName">Der eindeutige Name des Events.</param>
        public EventRegistry(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("Event name darf nicht leer sein.", nameof(eventName));

            EventName = eventName;
        }

        /// <summary>
        /// Registriert einen Handler.
        /// </summary>
        public virtual void Subscribe(IEventHandler<TEventArgs> handler)
        {
            if (handler == null)
                return;

            lock (_lockObject)
            {
                if (!_handlers.Contains(handler))
                    _handlers.Add(handler);
            }
        }

        /// <summary>
        /// Entfernt einen Handler.
        /// </summary>
        public virtual void Unsubscribe(IEventHandler<TEventArgs> handler)
        {
            if (handler == null)
                return;

            lock (_lockObject)
            {
                _handlers.Remove(handler);
            }
        }

        /// <summary>
        /// Registriert einen Listener.
        /// </summary>
        public virtual void Subscribe(IEventListener<TEventArgs> listener)
        {
            if (listener == null)
                return;

            lock (_lockObject)
            {
                if (!_listeners.Contains(listener))
                    _listeners.Add(listener);
            }
        }

        /// <summary>
        /// Entfernt einen Listener.
        /// </summary>
        public virtual void Unsubscribe(IEventListener<TEventArgs> listener)
        {
            if (listener == null)
                return;

            lock (_lockObject)
            {
                _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// Gibt alle registrierten Handler zurück.
        /// </summary>
        public virtual IReadOnlyCollection<IEventHandler<TEventArgs>> GetHandlers()
        {
            lock (_lockObject)
            {
                return new ReadOnlyCollection<IEventHandler<TEventArgs>>(_handlers.ToList());
            }
        }

        /// <summary>
        /// Gibt alle registrierten Listener zurück.
        /// </summary>
        public virtual IReadOnlyCollection<IEventListener<TEventArgs>> GetListeners()
        {
            lock (_lockObject)
            {
                return new ReadOnlyCollection<IEventListener<TEventArgs>>(_listeners.ToList());
            }
        }

        /// <summary>
        /// Publiziert ein Event synchron an alle Handler und Listener.
        /// </summary>
        public virtual void Publish(object sender, TEventArgs args)
        {
            List<IEventHandler<TEventArgs>> handlers;
            List<IEventListener<TEventArgs>> listeners;

            lock (_lockObject)
            {
                handlers = new List<IEventHandler<TEventArgs>>(_handlers);
                listeners = new List<IEventListener<TEventArgs>>(_listeners);
            }

            foreach (var handler in handlers)
            {
                try
                {
                    handler.Handle(sender, args);
                }
                catch
                {
                    // Fehler in einzelnen Handlern sollten andere nicht beeinflussen
                }
            }

            foreach (var listener in listeners)
            {
                try
                {
                    listener.Listen(sender, args);
                }
                catch
                {
                    // Fehler in einzelnen Listenern sollten andere nicht beeinflussen
                }
            }
        }

        /// <summary>
        /// Publiziert ein Event asynchron an alle Handler und Listener.
        /// </summary>
        public virtual async Task PublishAsync(object sender, TEventArgs args)
        {
            List<IEventHandler<TEventArgs>> handlers;
            List<IEventListener<TEventArgs>> listeners;

            lock (_lockObject)
            {
                handlers = new List<IEventHandler<TEventArgs>>(_handlers);
                listeners = new List<IEventListener<TEventArgs>>(_listeners);
            }

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                tasks.Add(handler.HandleAsync(sender, args).ContinueWith(_ =>
                {
                    // Fehler sollten Task nicht fehlschlagen lassen
                }, TaskScheduler.Default));
            }

            foreach (var listener in listeners)
            {
                tasks.Add(listener.ListenAsync(sender, args).ContinueWith(_ =>
                {
                    // Fehler sollten Task nicht fehlschlagen lassen
                }, TaskScheduler.Default));
            }

            await Task.WhenAll(tasks);
        }
    }
}
