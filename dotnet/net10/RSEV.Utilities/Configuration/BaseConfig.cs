using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RSEV.Utilities.Configuration
{
    /// <summary>
    /// Eine universelle, formatneutrale Basisklasse für Konfigurationsobjekte.
    /// Sie stellt die grundlegende Speicherlogik bereit und kann von
    /// spezifischen Implementierungen wie JSON-, YAML- oder CONF-Konfigurationen
    /// überschrieben und erweitert werden.
    /// </summary>
    public class BaseConfig : IConfig
    {
        /// <summary>
        /// Interner Speicher für alle Konfigurationswerte.
        /// </summary>
        protected readonly Dictionary<string, object> _values =
            new Dictionary<string, object>();

        /// <summary>
        /// Lädt die Konfiguration aus einer Datei oder Quelle.
        /// Diese Methode ist als Platzhalter gedacht und sollte von
        /// abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        public virtual void Load(string path)
        {
            // Wird von spezialisierten Klassen überschrieben.
        }

        /// <summary>
        /// Lädt die Konfiguration asynchron aus einer Datei oder Quelle.
        /// Diese Methode ist als Platzhalter gedacht und sollte von
        /// abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        public virtual Task LoadAsync(string path)
        {
            Load(path);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Speichert die Konfiguration in eine Datei oder Quelle.
        /// Diese Methode ist als Platzhalter gedacht und sollte von
        /// abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public virtual void Save(string path)
        {
            // Wird von spezialisierten Klassen überschrieben.
        }

        /// <summary>
        /// Speichert die Konfiguration asynchron in eine Datei oder Quelle.
        /// Diese Methode ist als Platzhalter gedacht und sollte von
        /// abgeleiteten Klassen überschrieben werden.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public virtual Task SaveAsync(string path)
        {
            Save(path);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Liest einen Wert aus der Konfiguration anhand eines Schlüssels.
        /// Falls der Schlüssel nicht existiert, wird der Standardwert von T zurückgegeben.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp des Wertes.</typeparam>
        /// <param name="key">Der Schlüssel, unter dem der Wert gespeichert ist.</param>
        /// <returns>Der gespeicherte Wert oder der Standardwert von T.</returns>
        public virtual T Get<T>(string key)
        {
            if (_values.TryGetValue(key, out var value))
            {
                if (value is T typed)
                    return typed;
            }

            return default;
        }

        /// <summary>
        /// Setzt oder überschreibt einen Wert in der Konfiguration.
        /// </summary>
        /// <typeparam name="T">Der Datentyp des zu speichernden Wertes.</typeparam>
        /// <param name="key">Der Schlüssel, unter dem der Wert gespeichert wird.</param>
        /// <param name="value">Der zu speichernde Wert.</param>
        public virtual void Set<T>(string key, T value)
        {
            _values[key] = value;
        }
    }
}
