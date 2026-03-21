using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace RSEV.Utilities.Configuration
{
    /// <summary>
    /// Eine einfache, leichtgewichtige Konfigurationsimplementierung,
    /// die das klassische "key=value"-Format verwendet. Ideal für
    /// kleine Projekte, Plugins, Server oder Umgebungen, in denen
    /// maximale Geschwindigkeit und Einfachheit benötigt werden.
    /// </summary>
    public class ConfConfig : BaseConfig
    {
        /// <summary>
        /// Erstellt eine neue Instanz der ConfConfig-Klasse.
        /// </summary>
        public ConfConfig() { }

        /// <summary>
        /// Lädt die Konfiguration aus einer CONF-Datei im Format "key=value".
        /// Leere Zeilen und Kommentare (beginnend mit '#') werden ignoriert.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        public override void Load(string path)
        {
            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                // Kommentare und leere Zeilen ignorieren
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split('=', 2);

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    _values[key] = value;
                }
            }
        }

        /// <summary>
        /// Lädt die Konfiguration asynchron aus einer CONF-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Konfigurationsdatei.</param>
        public override async Task LoadAsync(string path)
        {
            if (!File.Exists(path))
                return;

            var lines = await File.ReadAllLinesAsync(path);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split('=', 2);

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    _values[key] = value;
                }
            }
        }

        /// <summary>
        /// Speichert die Konfiguration in eine CONF-Datei im Format "key=value".
        /// Kommentare werden nicht automatisch generiert.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override void Save(string path)
        {
            var lines = new List<string>();

            foreach (var pair in _values)
            {
                lines.Add($"{pair.Key}={pair.Value}");
            }

            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// Speichert die Konfiguration asynchron in eine CONF-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override async Task SaveAsync(string path)
        {
            var lines = new List<string>();

            foreach (var pair in _values)
            {
                lines.Add($"{pair.Key}={pair.Value}");
            }

            await File.WriteAllLinesAsync(path, lines);
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
                return (T)Convert.ChangeType(raw, typeof(T));
            }
            catch
            {
                return default;
            }
        }

    }

}
