using RSEV.Utilities.Loading;
using RSEV.Utilities.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RSEV.Utilities.Plugins
{
    /// <summary>
    /// Ein globales Plugin-Register, das Plugins aus Assemblies lädt,
    /// instanziert, aktiviert, deaktiviert und neu laden kann. Ideal für
    /// modulare Systeme, Plugin-Architekturen und dynamische Erweiterungen.
    /// </summary>
    public class PluginRegistry
    {
        /// <summary>
        /// Der AssemblyLoader, der zum Laden von Plugin-Assemblies verwendet wird.
        /// </summary>
        protected readonly AssemblyLoader _loader = new AssemblyLoader();

        /// <summary>
        /// Die TypeDiscovery-Instanz, die zum Auffinden von Plugin-Typen verwendet wird.
        /// </summary>
        protected readonly TypeDiscovery _discovery = new TypeDiscovery();

        /// <summary>
        /// Eine Liste aller geladenen Plugin-Instanzen.
        /// </summary>
        protected readonly List<IPlugin> _plugins = new List<IPlugin>();

        /// <summary>
        /// Lädt alle Plugins aus einem Verzeichnis und aktiviert sie.
        /// </summary>
        /// <param name="directory">Das Verzeichnis, aus dem Plugins geladen werden sollen.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void LoadPlugins(string directory, IRuntimeContext context)
        {
            if (!Directory.Exists(directory))
                return;

            var assemblies = _loader.LoadAssembliesFromDirectory(directory);

            foreach (var asm in assemblies)
            {
                var types = _discovery.FindTypesImplementing<IPlugin>(asm);

                foreach (var type in types)
                {
                    var instance = _loader.CreateInstance<IPlugin>(type);

                    if (instance != null)
                    {
                        _plugins.Add(instance);
                        instance.OnEnable(context);
                    }
                }
            }
        }

        /// <summary>
        /// Lädt alle Plugins asynchron aus einem Verzeichnis und aktiviert sie.
        /// </summary>
        /// <param name="directory">Das Verzeichnis, aus dem Plugins geladen werden sollen.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task LoadPluginsAsync(string directory, IRuntimeContext context)
        {
            if (!Directory.Exists(directory))
                return;

            var assemblies = _loader.LoadAssembliesFromDirectory(directory);

            foreach (var asm in assemblies)
            {
                var types = _discovery.FindTypesImplementing<IPlugin>(asm);

                foreach (var type in types)
                {
                    var instance = _loader.CreateInstance<IPlugin>(type);

                    if (instance != null)
                    {
                        _plugins.Add(instance);
                        await instance.OnEnableAsync(context);
                    }
                }
            }
        }

        /// <summary>
        /// Deaktiviert alle geladenen Plugins.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void DisableAll(IRuntimeContext context)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnDisable(context);
            }
        }

        /// <summary>
        /// Deaktiviert alle geladenen Plugins asynchron.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task DisableAllAsync(IRuntimeContext context)
        {
            foreach (var plugin in _plugins)
            {
                await plugin.OnDisableAsync(context);
            }
        }

        /// <summary>
        /// Aktiviert alle geladenen Plugins erneut.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void EnableAll(IRuntimeContext context)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnEnable(context);
            }
        }

        /// <summary>
        /// Aktiviert alle geladenen Plugins asynchron erneut.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task EnableAllAsync(IRuntimeContext context)
        {
            foreach (var plugin in _plugins)
            {
                await plugin.OnEnableAsync(context);
            }
        }

        /// <summary>
        /// Führt einen vollständigen Reload aller Plugins durch: deaktivieren,
        /// neu laden, aktivieren. Ideal für Hot-Reloading oder Entwicklungsumgebungen.
        /// </summary>
        /// <param name="directory">Das Plugin-Verzeichnis.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void ForceReload(string directory, IRuntimeContext context)
        {
            DisableAll(context);
            _plugins.Clear();
            LoadPlugins(directory, context);
        }

        /// <summary>
        /// Führt einen vollständigen Reload aller Plugins asynchron durch.
        /// </summary>
        /// <param name="directory">Das Plugin-Verzeichnis.</param>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task ForceReloadAsync(string directory, IRuntimeContext context)
        {
            await DisableAllAsync(context);
            _plugins.Clear();
            await LoadPluginsAsync(directory, context);
        }

    }

}
