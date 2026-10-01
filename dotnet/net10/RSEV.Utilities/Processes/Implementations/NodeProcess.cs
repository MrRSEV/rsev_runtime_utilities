using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein Subprozess-Manager für Node.js-Skripte. Startet Node-Programme via "node &lt;script&gt;".
    /// </summary>
    public class NodeProcess : RuntimeSubprocessBase
    {
        /// <summary>
        /// Der Pfad zum Node.js-Skript.
        /// </summary>
        private readonly string _scriptPath;

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        private readonly string[] _arguments;

        /// <summary>
        /// Das ausführbare Programm ("node").
        /// </summary>
        public override string Executable => "node";

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        public override string[] Arguments => _arguments;

        /// <summary>
        /// Erstellt eine neue NodeProcess-Instanz.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="scriptPath">Der Pfad zum Node.js-Skript.</param>
        /// <param name="arguments">Optionale Kommandozeilen-Argumente.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public NodeProcess(string name, string scriptPath, string[]? arguments = null, ILogger? logger = null)
            : base(name, logger)
        {
            _scriptPath = scriptPath;
            _arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>
        /// Startet das Node.js-Skript.
        /// </summary>
        protected override async Task<bool> OnStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var args = string.Join(" ", new[] { $"\"{_scriptPath}\"" }.Concat(_arguments));

                var startInfo = new ProcessStartInfo
                {
                    FileName = Executable,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                _process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                _process.Start();

                PumpStdOut(_process);
                PumpStdErr(_process);

                await Task.Delay(500, cancellationToken);

                if (_process.HasExited)
                {
                    LogError($"Prozess wurde kurz nach dem Start beendet (ExitCode={_process.ExitCode})");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError($"Exception beim Start: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stoppt das Node.js-Skript.
        /// </summary>
        protected override async Task<bool> OnStopAsync()
        {
            if (_process == null || _process.HasExited)
            {
                return true;
            }

            try
            {
                _process.Kill(true);
                await _process.WaitForExitAsync();
                _process.Dispose();
                _process = null;
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Exception beim Stop: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Klassifiziert Log-Zeilen für Node.js-Prozesse.
        /// </summary>
        protected override ProcessLogLevel ClassifyLogLine(string line)
        {
            var normalized = line.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return ProcessLogLevel.Info;
            }

            // Heuristik für Node.js
            if (normalized.Contains("Error", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("FATAL", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("TypeError", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("SyntaxError", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Error;
            }

            if (normalized.Contains("Warning", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("WARN", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("DeprecationWarning", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Warning;
            }

            return ProcessLogLevel.Info;
        }
    }
}
