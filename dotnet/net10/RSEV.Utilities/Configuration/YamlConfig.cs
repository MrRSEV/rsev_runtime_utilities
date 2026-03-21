using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RSEV.Utilities.Configuration
{
    /// <summary>
    /// Eine leichtgewichtige YAML-Konfigurationsimplementierung, die ohne externe
    /// Bibliotheken auskommt. Unterstützt einfache Key-Value-Strukturen, Listen
    /// sowie verschachtelte Werte über Einrückung. Ideal für Tools, Plugins und
    /// serverseitige Konfigurationen.
    /// </summary>
    public class YamlConfig : BaseConfig
    {
        /// <summary>
        /// Erstellt eine neue Instanz der YamlConfig-Klasse.
        /// </summary>
        public YamlConfig() { }

        /// <summary>
        /// Lädt die YAML-Konfiguration aus einer Datei. Unterstützt einfache
        /// Key-Value-Paare, Listen und verschachtelte Strukturen.
        /// </summary>
        /// <param name="path">Der Pfad zur YAML-Datei.</param>
        public override void Load(string path)
        {
            if (!File.Exists(path))
                return;

            var lines = File.ReadAllLines(path);
            ParseYaml(lines);
        }

        /// <summary>
        /// Lädt die YAML-Konfiguration asynchron aus einer Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur YAML-Datei.</param>
        public override async Task LoadAsync(string path)
        {
            if (!File.Exists(path))
                return;

            var lines = await File.ReadAllLinesAsync(path);
            ParseYaml(lines);
        }

        /// <summary>
        /// Speichert die Konfiguration in eine YAML-Datei. Verschachtelte
        /// Strukturen werden nicht automatisch generiert, sondern flach
        /// im Key-Value-Format ausgegeben.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override void Save(string path)
        {
            var lines = new List<string>();

            foreach (var pair in _values)
            {
                lines.Add($"{pair.Key}: {pair.Value}");
            }

            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// Speichert die Konfiguration asynchron in eine YAML-Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Zieldatei.</param>
        public override async Task SaveAsync(string path)
        {
            var lines = new List<string>();

            foreach (var pair in _values)
            {
                lines.Add($"{pair.Key}: {pair.Value}");
            }

            await File.WriteAllLinesAsync(path, lines);
        }

        /// <summary>
        /// Liest einen Wert aus der YAML-Konfiguration und versucht,
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

        /// <summary>
        /// Parst eine YAML-Datei in das interne Dictionary. Unterstützt einfache
        /// Key-Value-Paare, Listen und verschachtelte Strukturen über Einrückung.
        /// </summary>
        /// <param name="lines">Die Zeilen der YAML-Datei.</param>
        protected virtual void ParseYaml(string[] lines)
        {
            _values.Clear();

            var stack = new Stack<string>();
            int previousIndent = 0;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                // Kommentare und leere Zeilen ignorieren
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                int indent = rawLine.Length - rawLine.TrimStart().Length;

                // Verschachtelung reduzieren
                while (indent < previousIndent && stack.Count > 0)
                {
                    stack.Pop();
                    previousIndent -= 2;
                }

                previousIndent = indent;

                // Listen-Element
                if (line.StartsWith("- "))
                {
                    var key = string.Join(".", stack);
                    var value = line.Substring(2).Trim();

                    if (!_values.ContainsKey(key))
                        _values[key] = new List<string>();

                    ((List<string>)_values[key]).Add(value);
                    continue;
                }

                // Key-Value
                var parts = line.Split(':', 2);

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Verschachtelter Schlüssel
                    if (stack.Count > 0)
                        key = string.Join(".", stack) + "." + key;

                    if (string.IsNullOrEmpty(value))
                    {
                        // Neuer verschachtelter Block
                        stack.Push(parts[0].Trim());
                    }
                    else
                    {
                        _values[key] = value;
                    }
                }

            }

        }

    }

}
