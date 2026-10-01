using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein Subprozess-Manager für .NET DLLs. Startet .NET-Programme via "dotnet &lt;dll&gt;".
    /// </summary>
    public class DotNetProcess : RuntimeSubprocessBase
    {
        /// <summary>
        /// Der Pfad zur .NET DLL.
        /// </summary>
        private readonly string _dllPath;

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        private readonly string[] _arguments;

        /// <summary>
        /// Das ausführbare Programm ("dotnet").
        /// </summary>
        public override string Executable => "dotnet";

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        public override string[] Arguments => _arguments;

        /// <summary>
        /// Erstellt eine neue DotNetProcess-Instanz.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="dllPath">Der Pfad zur .NET DLL.</param>
        /// <param name="arguments">Optionale Kommandozeilen-Argumente.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public DotNetProcess(string name, string dllPath, string[]? arguments = null, ILogger? logger = null)
            : base(name, logger)
        {
            _dllPath = dllPath;
            _arguments = arguments ?? Array.Empty<string>();
        }

        /// <summary>
        /// Startet die .NET DLL.
        /// </summary>
        protected override async Task<bool> OnStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var args = string.Join(" ", new[] { $"\"{_dllPath}\"" }.Concat(_arguments));

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

                // Kleine Verzögerung, um sicherzustellen, dass der Prozess tatsächlich läuft
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
        /// Stoppt die .NET DLL.
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
        /// Klassifiziert Log-Zeilen für .NET Prozesse.
        /// </summary>
        protected override ProcessLogLevel ClassifyLogLine(string line)
        {
            var normalized = line.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return ProcessLogLevel.Info;
            }

            // Einfache Heuristik für .NET
            if (normalized.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("exception", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("fatal", StringComparison.OrdinalIgnoreCase))
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
