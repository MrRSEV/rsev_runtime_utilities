using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein Subprozess-Manager für Shell-Kommandos. Kan bash (Linux) oder pwsh/cmd (Windows) ausführen.
    /// </summary>
    public class ShellProcess : RuntimeSubprocessBase
    {
        /// <summary>
        /// Das Shell-Kommando, das ausgeführt werden soll.
        /// </summary>
        private readonly string _command;

        /// <summary>
        /// Das ausführbare Programm (bash unter Linux, pwsh/cmd unter Windows).
        /// </summary>
        public override string Executable
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "pwsh" : "/bin/bash";
            }
        }

        /// <summary>
        /// Die Kommandozeilen-Argumente (Standard für Shell-Ausführung).
        /// </summary>
        public override string[] Arguments => new[] { "-Command", _command };

        /// <summary>
        /// Erstellt eine neue ShellProcess-Instanz.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="command">Das auszuführende Shell-Kommando.</param>
        /// <param name="logger">Optionaler Logger.</param>
        public ShellProcess(string name, string command, ILogger? logger = null)
            : base(name, logger)
        {
            _command = command;
        }

        /// <summary>
        /// Startet das Shell-Kommando.
        /// </summary>
        protected override async Task<bool> OnStartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Executable,
                    Arguments = string.Join(" ", Arguments.Select(a => $"\"{a}\"")),
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
                    LogInfo($"Shell-Kommando beendet (ExitCode={_process.ExitCode})");
                    return true; // Shell-Kommandos sind ok wenn sie beendet werden
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
        /// Stoppt das Shell-Kommando.
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
        /// Klassifiziert Log-Zeilen für Shell-Prozesse.
        /// </summary>
        protected override ProcessLogLevel ClassifyLogLine(string line)
        {
            var normalized = line.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return ProcessLogLevel.Info;
            }

            // Shell-Fehler-Heuristik
            if (normalized.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                normalized.Contains("failed", StringComparison.OrdinalIgnoreCase) ||
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
