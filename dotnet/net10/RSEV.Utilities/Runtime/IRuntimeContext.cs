using RSEV.Utilities.Configuration;
using RSEV.Utilities.Loading;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Messaging;
using RSEV.Utilities.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Runtime
{
    /// <summary>
    /// Definiert den zentralen Laufzeitkontext des Systems. Der RuntimeContext
    /// stellt globale Dienste, Konfigurationen, Loader, Discovery-Mechanismen
    /// und Statusinformationen bereit, die von Controllern, Plugins und
    /// Subsystemen gemeinsam genutzt werden können.
    /// </summary>
    public interface IRuntimeContext
    {
        /// <summary>
        /// Die globale Konfiguration des Systems. Kann beliebige Implementierungen
        /// wie JSON-, YAML-, CONF- oder benutzerdefinierte Configs enthalten.
        /// </summary>
        IConfig Config { get; }

        /// <summary>
        /// Der globale Logger, der von allen Systemkomponenten genutzt werden kann.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Der globale AssemblyLoader, der zum Laden von Assemblies und Plugins
        /// verwendet wird.
        /// </summary>
        AssemblyLoader AssemblyLoader { get; }

        /// <summary>
        /// Die globale TypeDiscovery-Instanz, die zum Auffinden von Typen,
        /// Controllern, Plugins oder Modulen verwendet wird.
        /// </summary>
        TypeDiscovery TypeDiscovery { get; }

        /// <summary>
        /// Das globale Plugin-Register, das Plugins lädt, aktiviert, deaktiviert
        /// und neu laden kann.
        /// </summary>
        PluginRegistry PluginRegistry { get; }

        /// <summary>
        /// Das globale MessageBus-Register, das Message Buses lädt, registriert
        /// und verwaltet.
        /// </summary>
        MessageBusRegistry MessageBusRegistry { get; }

        /// <summary>
        /// Ein universeller Key-Value-Speicher für globale Zustände, die von
        /// Controllern, Plugins oder Subsystemen gemeinsam genutzt werden können.
        /// </summary>
        IDictionary<string, object> GlobalState { get; }

        /// <summary>
        /// Gibt an, ob das System aktuell läuft oder sich im Shutdown befindet.
        /// </summary>
        bool IsRunning { get; set; }

        /// <summary>
        /// Registriert einen globalen Wert im Kontext.
        /// </summary>
        /// <param name="key">Der eindeutige Schlüssel.</param>
        /// <param name="value">Der zu speichernde Wert.</param>
        void Set(string key, object value);

        /// <summary>
        /// Liest einen globalen Wert aus dem Kontext.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp.</typeparam>
        /// <param name="key">Der Schlüssel des Wertes.</param>
        /// <returns>Der gespeicherte Wert oder der Standardwert von T.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Versucht, einen globalen Wert aus dem Kontext zu lesen.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp.</typeparam>
        /// <param name="key">Der Schlüssel des Wertes.</param>
        /// <param name="value">Der gelesene Wert, falls vorhanden.</param>
        /// <returns>True, wenn der Wert existiert, sonst false.</returns>
        bool TryGet<T>(string key, out T value);
    }
}
