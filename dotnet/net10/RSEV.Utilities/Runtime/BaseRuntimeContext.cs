using RSEV.Utilities.Configuration;
using RSEV.Utilities.Loading;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Runtime
{
    /// <summary>
    /// Eine universelle, erweiterbare Basisklasse für Runtime-Kontexte.
    /// Der BaseRuntimeContext stellt globale Dienste wie Konfiguration,
    /// AssemblyLoader, TypeDiscovery, PluginRegistry und einen globalen
    /// Key-Value-Speicher bereit. Er dient als Grundlage für spezialisierte
    /// Runtime-Kontexte wie MainRuntimeContext.
    /// </summary>
    public abstract class BaseRuntimeContext : IRuntimeContext
    {
        /// <summary>
        /// Die globale Konfiguration des Systems.
        /// </summary>
        public virtual IConfig Config { get; protected set; }

        /// <summary>
        /// Der globale AssemblyLoader, der zum Laden von Assemblies verwendet wird.
        /// </summary>
        public virtual AssemblyLoader AssemblyLoader { get; protected set; }

        /// <summary>
        /// Die globale TypeDiscovery-Instanz, die zum Auffinden von Typen verwendet wird.
        /// </summary>
        public virtual TypeDiscovery TypeDiscovery { get; protected set; }

        /// <summary>
        /// Das globale Plugin-Register, das Plugins lädt, aktiviert und deaktiviert.
        /// </summary>
        public virtual PluginRegistry PluginRegistry { get; protected set; }

        /// <summary>
        /// Ein universeller Key-Value-Speicher für globale Zustände.
        /// </summary>
        public virtual IDictionary<string, object> GlobalState { get; protected set; }

        /// <summary>
        /// Gibt an, ob das System aktuell läuft oder sich im Shutdown befindet.
        /// </summary>
        public virtual bool IsRunning { get; set; }

        /// <summary>
        /// Der globale Logger, der allen Systemkomponenten zur Verfügung steht.
        /// Über diese Instanz können Controller, Plugins und Subsysteme
        /// konsistente Log-Ausgaben erzeugen.
        /// </summary>
        public virtual ILogger Logger { get; protected set; }

        /// <summary>
        /// Erstellt die Standard-Logger-Instanz für den RuntimeContext.
        /// Abgeleitete Klassen können diese Methode überschreiben, um
        /// alternative Logger bereitzustellen, z. B. CompositeLogger,
        /// RemoteLogger oder projektspezifische Implementierungen.
        /// </summary>
        /// <returns>Eine neue Instanz eines ILogger.</returns>
        protected virtual ILogger CreateLogger()
        {
            return new SystemLog();
        }

        /// <summary>
        /// Erstellt eine neue Instanz des BaseRuntimeContext und initialisiert
        /// alle globalen Dienste. Abgeleitete Klassen können diese Werte
        /// überschreiben oder erweitern.
        /// </summary>
        protected BaseRuntimeContext()
        {
            Config = CreateDefaultConfig();
            AssemblyLoader = CreateAssemblyLoader();
            TypeDiscovery = CreateTypeDiscovery();
            PluginRegistry = CreatePluginRegistry();
            Logger = CreateLogger();
            GlobalState = new Dictionary<string, object>();
            IsRunning = false;
        }

        /// <summary>
        /// Erstellt die Standardkonfiguration. Abgeleitete Klassen können
        /// diese Methode überschreiben, um eigene Config-Implementierungen
        /// zu verwenden.
        /// </summary>
        /// <returns>Eine neue IConfig-Instanz.</returns>
        protected virtual IConfig CreateDefaultConfig()
        {
            return new ConfConfig();
        }

        /// <summary>
        /// Erstellt den Standard-AssemblyLoader. Abgeleitete Klassen können
        /// diese Methode überschreiben, um eigene Loader bereitzustellen.
        /// </summary>
        /// <returns>Eine neue AssemblyLoader-Instanz.</returns>
        protected virtual AssemblyLoader CreateAssemblyLoader()
        {
            return new AssemblyLoader();
        }

        /// <summary>
        /// Erstellt die Standard-TypeDiscovery. Abgeleitete Klassen können
        /// diese Methode überschreiben, um eigene Discovery-Mechanismen
        /// zu verwenden.
        /// </summary>
        /// <returns>Eine neue TypeDiscovery-Instanz.</returns>
        protected virtual TypeDiscovery CreateTypeDiscovery()
        {
            return new TypeDiscovery();
        }

        /// <summary>
        /// Erstellt das Standard-PluginRegistry. Abgeleitete Klassen können
        /// diese Methode überschreiben, um eigene Plugin-Systeme zu integrieren.
        /// </summary>
        /// <returns>Eine neue PluginRegistry-Instanz.</returns>
        protected virtual PluginRegistry CreatePluginRegistry()
        {
            return new PluginRegistry();
        }

        /// <summary>
        /// Registriert einen globalen Wert im Kontext.
        /// </summary>
        /// <param name="key">Der eindeutige Schlüssel.</param>
        /// <param name="value">Der zu speichernde Wert.</param>
        public virtual void Set(string key, object value)
        {
            GlobalState[key] = value;
        }

        /// <summary>
        /// Liest einen globalen Wert aus dem Kontext.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp.</typeparam>
        /// <param name="key">Der Schlüssel des Wertes.</param>
        /// <returns>Der gespeicherte Wert oder der Standardwert von T.</returns>
        public virtual T Get<T>(string key)
        {
            if (GlobalState.TryGetValue(key, out var value))
            {
                if (value is T typed)
                    return typed;
            }

            return default;
        }

        /// <summary>
        /// Versucht, einen globalen Wert aus dem Kontext zu lesen.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp.</typeparam>
        /// <param name="key">Der Schlüssel des Wertes.</param>
        /// <param name="value">Der gelesene Wert, falls vorhanden.</param>
        /// <returns>True, wenn der Wert existiert, sonst false.</returns>
        public virtual bool TryGet<T>(string key, out T value)
        {
            if (GlobalState.TryGetValue(key, out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }

            value = default;
            return false;
        }

    }

}
