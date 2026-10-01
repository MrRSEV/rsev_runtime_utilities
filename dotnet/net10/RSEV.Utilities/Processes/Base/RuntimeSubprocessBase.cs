using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Eine abstrakte Basisklasse für verwaltete Subprozesse. Sie verwaltet den Lifecycle,
    /// bietet Logging-Methoden und implementiert Standard-Verhalten.
    /// </summary>
    public abstract class RuntimeSubprocessBase : IRuntimeSubprocess
    {
        /// <summary>
        /// Der interne System-Prozess.
        /// </summary>
        protected Process? _process;

        /// <summary>
        /// Die eindeutige ID des Prozesses (UUID-basiert).
        /// </summary>
        public string ProcessId { get; }

        /// <summary>
        /// Der Name des Prozesses.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Das ausführbare Programm.
        /// </summary>
        public abstract string Executable { get; }

        /// <summary>
        /// Die Kommandozeilen-Argumente.
        /// </summary>
        public abstract string[] Arguments { get; }

        /// <summary>
        /// Der aktuelle Zustand des Prozesses.
        /// </summary>
        public SubprocessState State { get; protected set; }

        /// <summary>
        /// Der Exit-Code des Prozesses.
        /// </summary>
        public int? ExitCode => _process?.ExitCode;

        /// <summary>
        /// Ein Key-Value-Speicher für prozesstyp-spezifische Eigenschaften.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Event für stdout-Ausgaben.
        /// </summary>
        public event Action<IRuntimeSubprocess, string>? OnStdOut;

        /// <summary>
        /// Event für stderr-Ausgaben.
        /// </summary>
        public event Action<IRuntimeSubprocess, string>? OnStdErr;

        /// <summary>
        /// Das globale Logger-Objekt (optional, aus RuntimeContext).
        /// </summary>
        protected ILogger? _logger;

        /// <summary>
        /// Erstellt eine neue Instanz einer RuntimeSubprocess.
        /// </summary>
        /// <param name="name">Der Name des Prozesses.</param>
        /// <param name="logger">Optionaler Logger für Ausgaben.</param>
        protected RuntimeSubprocessBase(string name, ILogger? logger = null)
        {
            ProcessId = Guid.NewGuid().ToString("N").Substring(0, 12);
            Name = name;
            Properties = new Dictionary<string, object>();
            State = SubprocessState.Created;
            _logger = logger;
        }

        /// <summary>
        /// Startet den Prozess asynchron. Ruft intern OnStartAsync auf.
        /// </summary>
        public virtual async Task<bool> StartAsync(CancellationToken cancellationToken = default)
        {
            if (State == SubprocessState.Running)
            {
                LogWarning($"[{Name}] Prozess läuft bereits.");
                return true;
            }

            State = SubprocessState.Starting;

            try
            {
                var result = await OnStartAsync(cancellationToken);

                if (result)
                {
                    State = SubprocessState.Running;
                    LogInfo($"[{Name}] Prozess gestartet (PID: {ProcessId})");
                    return true;
                }

                State = SubprocessState.Faulted;
                LogError($"[{Name}] Fehler beim Starten des Prozesses.");
                return false;
            }
            catch (Exception ex)
            {
                State = SubprocessState.Faulted;
                LogError($"[{Name}] Exception beim Start: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stoppt den Prozess asynchron. Ruft intern OnStopAsync auf.
        /// </summary>
        public virtual async Task<bool> StopAsync()
        {
            if (State != SubprocessState.Running)
            {
                LogWarning($"[{Name}] Prozess ist nicht am Laufen.");
                return true;
            }

            State = SubprocessState.Stopping;

            try
            {
                var result = await OnStopAsync();

                if (result)
                {
                    State = SubprocessState.Stopped;
                    LogInfo($"[{Name}] Prozess beendet.");
                    return true;
                }

                State = SubprocessState.Faulted;
                LogError($"[{Name}] Fehler beim Beenden des Prozesses.");
                return false;
            }
            catch (Exception ex)
            {
                State = SubprocessState.Faulted;
                LogError($"[{Name}] Exception beim Stop: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Startet den Prozess neu asynchron.
        /// </summary>
        public virtual async Task<bool> RestartAsync(CancellationToken cancellationToken = default)
        {
            await StopAsync();
            await Task.Delay(500, cancellationToken);
            return await StartAsync(cancellationToken);
        }

        /// <summary>
        /// Prüft die Gesundheit des Prozesses. Default: prüft ob laufend.
        /// </summary>
        public virtual Task<bool?> CheckHealthAsync()
        {
            return Task.FromResult<bool?>(State == SubprocessState.Running);
        }

        /// <summary>
        /// Hook-Methode für die Start-Logik. Überschreiben Sie diese.
        /// </summary>
        protected abstract Task<bool> OnStartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Hook-Methode für die Stop-Logik. Überschreiben Sie diese.
        /// </summary>
        protected abstract Task<bool> OnStopAsync();

        /// <summary>
        /// Klassifiziert eine Log-Zeile nach ProcessLogLevel. Muss von konkreten Klassen überschrieben werden.
        /// </summary>
        protected abstract ProcessLogLevel ClassifyLogLine(string line);

        /// <summary>
        /// Protokolliert eine Information.
        /// </summary>
        protected void LogInfo(string message)
        {
            _logger?.LogInfo(message);
        }

        /// <summary>
        /// Protokolliert eine Warnung.
        /// </summary>
        protected void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }

        /// <summary>
        /// Protokolliert einen Fehler.
        /// </summary>
        protected void LogError(string message)
        {
            _logger?.LogError(message);
        }

        /// <summary>
        /// Protokolliert eine Nachricht nach level.
        /// </summary>
        protected void LogByLevel(ProcessLogLevel level, string message)
        {
            if (_logger == null) return;

            switch (level)
            {
                case ProcessLogLevel.Info:
                    _logger.LogInfo(message);
                    break;
                case ProcessLogLevel.Warning:
                    _logger.LogWarning(message);
                    break;
                case ProcessLogLevel.Error:
                    _logger.LogError(message);
                    break;
            }
        }

        /// <summary>
        /// Ruft das OnStdOut Event auf.
        /// </summary>
        protected void RaiseStdOut(string line)
        {
            OnStdOut?.Invoke(this, line);
        }

        /// <summary>
        /// Ruft das OnStdErr Event auf.
        /// </summary>
        protected void RaiseStdErr(string line)
        {
            OnStdErr?.Invoke(this, line);
        }

        /// <summary>
        /// Pumpt stdout eines Prozesses kontinuierlich.
        /// </summary>
        protected void PumpStdOut(Process process)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!process.HasExited)
                    {
                        var line = await process.StandardOutput.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var level = ClassifyLogLine(line);
                            LogByLevel(level, line);
                            RaiseStdOut(line);
                        }
                    }
                }
                catch { }
            });
        }

        /// <summary>
        /// Pumpt stderr eines Prozesses kontinuierlich.
        /// </summary>
        protected void PumpStdErr(Process process)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!process.HasExited)
                    {
                        var line = await process.StandardError.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var level = ClassifyLogLine(line);
                            LogByLevel(level, line);
                            RaiseStdErr(line);
                        }
                    }
                }
                catch { }
            });
        }
    }
}
