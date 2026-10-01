using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein Subprozess-Manager für Python-Skripte. Startet Python-Programme via "python &lt;script&gt;" oder "python3".
    /// </summary>
    public class PythonProcess : RuntimeSubprocessBase
    {
        /// <summary>
        /// Der Pfad zum Python-Skript.
        /// </summary>
        private readonly string _scriptPath;

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        private readonly string[] _arguments;

        /// <summary>
        /// Das ausführbare Programm ("python" oder "python3").
        /// </summary>
        public override string Executable => "python";

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        public override string[] Arguments => _arguments;

        /// <summary>
        /// Erstellt eine neue PythonProcess-Instanz.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="scriptPath">Der Pfad zum Python-Skript.</param>
        /// <param name="arguments">Optionale Kommandozeilen-Argumente.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public PythonProcess(string name, string scriptPath, string[]? arguments = null, ILogger? logger = null)
            : base(name, logger)
        {
            _scriptPath = scriptPath;
            _arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>
        /// Startet das Python-Skript.
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
        /// Stoppt das Python-Skript.
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
        /// Klassifiziert Log-Zeilen für Python-Prozesse.
        /// </summary>
        protected override ProcessLogLevel ClassifyLogLine(string line)
        {
            var normalized = line.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return ProcessLogLevel.Info;
            }

            // Python-Fehler-Heuristik
            if (normalized.Contains("Error", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("Exception", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("Traceback", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("CRITICAL", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Error;
            }

            if (normalized.Contains("Warning", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("DeprecationWarning", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("WARNING", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Warning;
            }

            return ProcessLogLevel.Info;
        }
    }
}
