using RSEV.Utilities.Loading;
using RSEV.Utilities.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RSEV.Utilities.Messaging
{
    /// <summary>
    /// Ein globales MessageBus-Register, das Message Buses aus Assemblies lädt,
    /// instanziert, registriert und verwaltet. Ideal für modulare Systeme und
    /// benutzerdefinierte Messaging-Architekturen.
    /// </summary>
    public class MessageBusRegistry
    {
        /// <summary>
        /// Der AssemblyLoader, der zum Laden von Message-Bus-Assemblies verwendet wird.
        /// </summary>
        protected readonly AssemblyLoader _loader = new AssemblyLoader();

        /// <summary>
        /// Die TypeDiscovery-Instanz, die zum Auffinden von Message-Bus-Typen verwendet wird.
        /// </summary>
        protected readonly TypeDiscovery _discovery = new TypeDiscovery();

        /// <summary>
        /// Eine Liste aller registrierten Message-Bus-Instanzen.
        /// </summary>
        protected readonly List<IMessageBus> _buses = new List<IMessageBus>();

        /// <summary>
        /// Registriert einen Message Bus.
        /// </summary>
        /// <param name="bus">Der zu registrierende Message Bus.</param>
        public virtual void Register(IMessageBus bus)
        {
            if (bus == null || string.IsNullOrWhiteSpace(bus.Name))
                return;

            _buses.RemoveAll(x => string.Equals(x.Name, bus.Name, StringComparison.OrdinalIgnoreCase));
            _buses.Add(bus);
        }

        /// <summary>
        /// Registriert mehrere Message Buses auf einmal.
        /// </summary>
        /// <param name="buses">Die zu registrierenden Message Buses.</param>
        public virtual void RegisterRange(IEnumerable<IMessageBus> buses)
        {
            foreach (var bus in buses)
                Register(bus);
        }

        /// <summary>
        /// Prüft, ob ein Message Bus mit dem angegebenen Namen existiert.
        /// </summary>
        /// <param name="name">Der Name des Message Busses.</param>
        /// <returns>True, wenn der Message Bus existiert, sonst false.</returns>
        public virtual bool Exists(string name)
        {
            return Get(name) != null;
        }

        /// <summary>
        /// Holt einen registrierten Message Bus anhand seines Namens.
        /// </summary>
        /// <param name="name">Der Name des Message Busses.</param>
        /// <returns>Der Message Bus oder null, falls nicht vorhanden.</returns>
        public virtual IMessageBus Get(string name)
        {
            return _buses.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gibt alle registrierten Message Buses zurück.
        /// </summary>
        /// <returns>Alle registrierten Message Buses.</returns>
        public virtual IEnumerable<IMessageBus> GetAll()
        {
            return _buses;
        }

        /// <summary>
        /// Lädt alle Message Buses aus einem Verzeichnis und initialisiert sie.
        /// </summary>
        /// <param name="directory">Das Verzeichnis, aus dem Message Buses geladen werden sollen.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void LoadMessageBuses(string directory, IRuntimeContext context)
        {
            if (!Directory.Exists(directory))
                return;

            var assemblies = _loader.LoadAssembliesFromDirectory(directory);

            foreach (var asm in assemblies)
            {
                var types = _discovery.FindTypesImplementing<IMessageBus>(asm);

                foreach (var type in types)
                {
                    var instance = _loader.CreateInstance<IMessageBus>(type);

                    if (instance != null)
                    {
                        Register(instance);
                        instance.Initialize(context);
                    }
                }
            }
        }

        /// <summary>
        /// Lädt alle Message Buses asynchron aus einem Verzeichnis und initialisiert sie.
        /// </summary>
        /// <param name="directory">Das Verzeichnis, aus dem Message Buses geladen werden sollen.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task LoadMessageBusesAsync(string directory, IRuntimeContext context)
        {
            if (!Directory.Exists(directory))
                return;

            var assemblies = _loader.LoadAssembliesFromDirectory(directory);

            foreach (var asm in assemblies)
            {
                var types = _discovery.FindTypesImplementing<IMessageBus>(asm);

                foreach (var type in types)
                {
                    var instance = _loader.CreateInstance<IMessageBus>(type);

                    if (instance != null)
                    {
                        Register(instance);
                        await instance.InitializeAsync(context);
                    }
                }
            }
        }

        /// <summary>
        /// Fährt alle registrierten Message Buses herunter.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void ShutdownAll(IRuntimeContext context)
        {
            foreach (var bus in _buses)
            {
                bus.Shutdown(context);
            }
        }

        /// <summary>
        /// Fährt alle registrierten Message Buses asynchron herunter.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task ShutdownAllAsync(IRuntimeContext context)
        {
            foreach (var bus in _buses)
            {
                await bus.ShutdownAsync(context);
            }
        }

    }
}
