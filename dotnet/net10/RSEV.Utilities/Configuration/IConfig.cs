using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Configuration
{
    /// <summary>
    /// Definiert die grundlegenden Funktionen für ein universelles Konfigurationsobjekt.
    /// Implementierungen können beliebige Formate wie JSON, YAML, CONF oder eigene
    /// Strukturen verwenden. Das Interface ist bewusst minimal gehalten, um maximale
    /// Flexibilität und Erweiterbarkeit zu gewährleisten.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Lädt die Konfiguration aus einer Datei oder Quelle.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        void Load(string path);

        /// <summary>
        /// Lädt die Konfiguration asynchron aus einer Datei oder Quelle.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        Task LoadAsync(string path);

        /// <summary>
        /// Speichert die Konfiguration in eine Datei oder Quelle.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        void Save(string path);

        /// <summary>
        /// Speichert die Konfiguration asynchron in eine Datei oder Quelle.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        Task SaveAsync(string path);

        /// <summary>
        /// Liest einen Wert aus der Konfiguration anhand eines Schlüssels.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp des Wertes.</typeparam>
        /// <param name="key">Der Schlüssel, unter dem der Wert gespeichert ist.</param>
        /// <returns>Der gelesene Wert oder der Standardwert von T.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Setzt oder überschreibt einen Wert in der Konfiguration.
        /// </summary>
        /// <typeparam name="T">Der Datentyp des zu speichernden Wertes.</typeparam>
        /// <param name="key">Der Schlüssel, unter dem der Wert gespeichert wird.</param>
        /// <param name="value">Der zu speichernde Wert.</param>
        void Set<T>(string key, T value);
    }
}
