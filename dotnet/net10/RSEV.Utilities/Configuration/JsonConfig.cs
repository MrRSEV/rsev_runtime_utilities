using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RSEV.Utilities.Configuration
{
    /// <summary>
    /// Eine JSON-basierte Konfigurationsimplementierung, die auf System.Text.Json
    /// aufbaut. Sie unterstützt das Laden und Speichern von Konfigurationswerten
    /// in JSON-Dateien und nutzt intern ein Dictionary für schnellen Zugriff.
    /// </summary>
    public class JsonConfig : BaseConfig
    {
        /// <summary>
        /// Optionen für den JSON-Serializer, z. B. für Pretty-Print oder
        /// benutzerdefinierte Konvertierungen.
        /// </summary>
        protected virtual JsonSerializerOptions SerializerOptions { get; }
            = new JsonSerializerOptions
            {
                WriteIndented = true
            };

        /// <summary>
        /// Erstellt eine neue Instanz der JsonConfig-Klasse.
        /// </summary>
        public JsonConfig() { }

        /// <summary>
        /// Lädt die Konfiguration aus einer JSON-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur JSON-Konfigurationsdatei.</param>
        public override void Load(string path)
        {
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, SerializerOptions);

            if (dict == null)
                return;

            _values.Clear();

            foreach (var pair in dict)
            {
                _values[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Lädt die Konfiguration asynchron aus einer JSON-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur JSON-Konfigurationsdatei.</param>
        public override async Task LoadAsync(string path)
        {
            if (!File.Exists(path))
                return;

            var json = await File.ReadAllTextAsync(path);

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, SerializerOptions);

            if (dict == null)
                return;

            _values.Clear();

            foreach (var pair in dict)
            {
                _values[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Speichert die Konfiguration in eine JSON-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override void Save(string path)
        {
            var json = JsonSerializer.Serialize(_values, SerializerOptions);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Speichert die Konfiguration asynchron in eine JSON-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override async Task SaveAsync(string path)
        {
            var json = JsonSerializer.Serialize(_values, SerializerOptions);
            await File.WriteAllTextAsync(path, json);
        }

        /// <summary>
        /// Liest einen Wert aus der Konfiguration und versucht,
        /// ihn in den gewünschten Typ T zu konvertieren.
        /// </summary>
        /// <typeparam name="T">Der erwartete Datentyp.</typeparam>
        /// <param name="key">Der Schlüssel des Wertes.</param>
        /// <returns>Der konvertierte Wert oder der Standardwert von T.</returns>
        public override T Get<T>(string key)
        {
            if (!_values.TryGetValue(key, out var raw))
                return default;

            try
            {
                // JSON speichert Zahlen als JsonElement → Konvertierung nötig
                if (raw is JsonElement element)
                {
                    return JsonSerializer.Deserialize<T>(element.GetRawText(), SerializerOptions);
                }

                return (T)Convert.ChangeType(raw, typeof(T));
            }
            catch
            {
                return default;
            }
        }

    }

}
