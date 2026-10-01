using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein generischer Subprozess-Manager für beliebige ausführbare Dateien.
    /// Nutzt ExecutableResolver für Cross-Platform-Auflösung.
    /// </summary>
    public class ExecutableProcess : RuntimeSubprocessBase
    {
        /// <summary>
        /// Der aufgelöste Pfad zum Executable.
        /// </summary>
        private readonly string _resolvedExecutable;

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        private readonly string[] _arguments;

        /// <summary>
        /// Das ausführbare Programm (als aufgelöst gespeichert).
        /// </summary>
        public override string Executable => _resolvedExecutable;

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        public override string[] Arguments => _arguments;

        /// <summary>
        /// Erstellt eine neue ExecutableProcess-Instanz mit manueller Pfadangabe.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="executablePath">Der Pfad zum ausführbaren Programm.</param>
        /// <param name="arguments">Optionale Kommandozeilen-Argumente.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public ExecutableProcess(
            string name,
            string executablePath,
            string[]? arguments = null,
            ILogger? logger = null)
            : base(name, logger)
        {
            _resolvedExecutable = executablePath;
            _arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>
        /// Erstellt eine neue ExecutableProcess-Instanz mit Resolver-Auflösung.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="environmentVariable">Name der Umgebungsvariable für das Executable.</param>
        /// <param name="windowsPath">Fallback-Pfad unter Windows.</param>
        /// <param name="linuxPath">Fallback-Pfad unter Linux (System-Kommando oder absolut).</param>
        /// <param name="arguments">Optionale Kommandozeilen-Argumente.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public ExecutableProcess(
            string name,
            string environmentVariable,
            string windowsPath,
            string linuxPath,
            string[]? arguments = null,
            ILogger? logger = null)
            : base(name, logger)
        {
            _resolvedExecutable = ExecutableResolver.Resolve(environmentVariable, windowsPath, linuxPath);
            _arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>
        /// Startet das Executable.
        /// </summary>
        protected override async Task<bool> OnStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Unter Linux: Stelle sicher, dass die Datei ausführbar ist
                ExecutableResolver.EnsureExecutablePermissions(_resolvedExecutable);

                var args = string.Join(" ", _arguments.Select(a => $"\"{a}\""));

                var startInfo = new ProcessStartInfo
                {
                    FileName = _resolvedExecutable,
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
        /// Stoppt das Executable.
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
        /// Klassifiziert Log-Zeilen für generic Executables.
        /// </summary>
        protected override ProcessLogLevel ClassifyLogLine(string line)
        {
            var normalized = line.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return ProcessLogLevel.Info;
            }

            // Generische Heuristik
            if (normalized.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("fatal", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("fail", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Error;
            }

            if (normalized.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("warn", StringComparison.OrdinalIgnoreCase))
            {
                return ProcessLogLevel.Warning;
            }

            return ProcessLogLevel.Info;
        }
    }
}
