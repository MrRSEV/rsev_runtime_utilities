using RSEV.Utilities.Messaging;
using RSEV.Utilities.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Eine abstrakte Basisklasse für Event Buses. Sie implementiert IEventBus
    /// und verwaltet mehrere Event-Registries zentral. Framework-Nutzer können
    /// diese Klasse erweitern und als MessageBus registrieren.
    /// </summary>
    public abstract class BaseEventBus : BaseMessageBus, IEventBus
    {
        /// <summary>
        /// Ein Dictionary, das Event-Namen auf generische Registries abbildet.
        /// Der Schlüssel ist "EventName_TypeName" zur Eindeutigkeit.
        /// </summary>
        protected readonly Dictionary<string, object> _registries = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Ein Lock-Objekt für Thread-Safety beim Zugriff auf Registries.
        /// </summary>
        protected readonly object _lockObject = new();

        /// <summary>
        /// Erstellt eine neue Instanz eines BaseEventBus.
        /// </summary>
        /// <param name="name">Der eindeutige Name des Event Buses.</param>
        /// <param name="description">Die Beschreibung des Event Buses.</param>
        protected BaseEventBus(string name, string description = null)
            : base(name, description)
        {
        }

        /// <summary>
        /// Registriert eine Event-Registry für einen Event-Typ.
        /// </summary>
        public virtual void RegisterEventRegistry<TEventArgs>(IEventRegistry<TEventArgs> registry)
            where TEventArgs : EventArgs
        {
            if (registry == null || string.IsNullOrWhiteSpace(registry.EventName))
                return;

            string key = BuildRegistryKey(registry.EventName, typeof(TEventArgs));

            lock (_lockObject)
            {
                _registries[key] = registry;
            }
        }

        /// <summary>
        /// Ruft eine registrierte Event-Registry ab.
        /// </summary>
        public virtual IEventRegistry<TEventArgs> GetRegistry<TEventArgs>(string eventName)
            where TEventArgs : EventArgs
        {
            if (string.IsNullOrWhiteSpace(eventName))
                return null;

            string key = BuildRegistryKey(eventName, typeof(TEventArgs));

            lock (_lockObject)
            {
                if (_registries.TryGetValue(key, out var registry))
                    return registry as IEventRegistry<TEventArgs>;
            }

            return null;
        }

        /// <summary>
        /// Prüft, ob eine Event-Registry existiert.
        /// </summary>
        public virtual bool HasRegistry<TEventArgs>(string eventName)
            where TEventArgs : EventArgs
        {
            return GetRegistry<TEventArgs>(eventName) != null;
        }

        /// <summary>
        /// Gibt alle registrierten Event-Namen zurück.
        /// </summary>
        public virtual IEnumerable<string> GetAllEventNames()
        {
            lock (_lockObject)
            {
                return _registries.Keys
                    .Select(k => k.Split('_')[0])
                    .Distinct()
                    .ToList();
            }
        }

        /// <summary>
        /// Erstellt einen eindeutigen Schlüssel für eine Registry basierend auf Event-Name und Typ.
        /// </summary>
        protected virtual string BuildRegistryKey(string eventName, Type eventArgsType)
        {
            return $"{eventName}_{eventArgsType.FullName}";
        }
    }
}
