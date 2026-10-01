using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Verwaltet eine zentrale Sammlung von Subprozessen. Bietet Lifecycle- und Discovery-Methoden.
    /// </summary>
    public class RuntimeProcessController
    {
        /// <summary>
        /// Der Speicher für registrierte Prozesse.
        /// </summary>
        private readonly Dictionary<string, IRuntimeSubprocess> _processes;

        /// <summary>
        /// Lock für Thread-Sicherheit.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Optionaler Logger für Controller-Ereignisse.
        /// </summary>
        private readonly ILogger? _logger;

        /// <summary>
        /// Erstellt einen neuen RuntimeProcessController.
        /// </summary>
        /// <param name="logger">Optionaler Logger.</param>
        public RuntimeProcessController(ILogger? logger = null)
        {
            _processes = new Dictionary<string, IRuntimeSubprocess>();
            _logger = logger;
        }

        /// <summary>
        /// Registriert einen neuen Subprozess.
        /// </summary>
        /// <param name="process">Der zu registrierende Subprozess.</param>
        /// <returns>True wenn erfolgreich registriert, false wenn bereits registriert.</returns>
        public bool Register(IRuntimeSubprocess process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            lock (_lock)
            {
                if (_processes.ContainsKey(process.ProcessId))
                {
                    LogWarning($"Prozess '{process.Name}' (ID: {process.ProcessId}) ist bereits registriert.");
                    return false;
                }

                _processes[process.ProcessId] = process;
                LogInfo($"Prozess '{process.Name}' (ID: {process.ProcessId}) registriert.");
                return true;
            }
        }

        /// <summary>
        /// Registriert mehrere Subprozesse.
        /// </summary>
        /// <param name="processes">Die zu registrierenden Subprozesse.</param>
        /// <returns>Anzahl der erfolgreich registrierten Prozesse.</returns>
        public int RegisterRange(IEnumerable<IRuntimeSubprocess> processes)
        {
            int count = 0;
            foreach (var process in processes)
            {
                if (Register(process))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Prüft, ob ein Prozess mit gegebener ID existiert.
        /// </summary>
        public bool Exists(string processId)
        {
            lock (_lock)
            {
                return _processes.ContainsKey(processId);
            }
        }

        /// <summary>
        /// Ruft einen registrierten Prozess nach ID auf.
        /// </summary>
        /// <param name="processId">Die ID des Prozesses.</param>
        /// <returns>Der Prozess, oder null wenn nicht gefunden.</returns>
        public IRuntimeSubprocess? Get(string processId)
        {
            lock (_lock)
            {
                _processes.TryGetValue(processId, out var process);
                return process;
            }
        }

        /// <summary>
        /// Ruft alle registrierten Prozesse auf.
        /// </summary>
        public IEnumerable<IRuntimeSubprocess> GetAll()
        {
            lock (_lock)
            {
                return _processes.Values.ToList();
            }
        }

        /// <summary>
        /// Findet Prozesse nach Name.
        /// </summary>
        public IEnumerable<IRuntimeSubprocess> FindByName(string name)
        {
            lock (_lock)
            {
                return _processes.Values
                    .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        /// <summary>
        /// Findet Prozesse nach Zustand.
        /// </summary>
        public IEnumerable<IRuntimeSubprocess> FindByState(SubprocessState state)
        {
            lock (_lock)
            {
                return _processes.Values
                    .Where(p => p.State == state)
                    .ToList();
            }
        }

        /// <summary>
        /// Entfernt einen Prozess aus der Registry.
        /// </summary>
        /// <param name="processId">Die ID des Prozesses.</param>
        /// <returns>True wenn erfolgreich entfernt, false wenn nicht gefunden.</returns>
        public bool Unregister(string processId)
        {
            lock (_lock)
            {
                if (_processes.TryGetValue(processId, out var process))
                {
                    _processes.Remove(processId);
                    LogInfo($"Prozess '{process.Name}' (ID: {processId}) entfernt.");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Startet einen Prozess asynchron.
        /// </summary>
        public async Task<bool> StartAsync(string processId, CancellationToken cancellationToken = default)
        {
            var process = Get(processId);
            if (process == null)
            {
                LogWarning($"Prozess '{processId}' nicht gefunden.");
                return false;
            }

            return await process.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Konvertiert einen Namen in eine Prozess-ID und startet den Prozess.
        /// </summary>
        public async Task<bool> StartByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var processes = FindByName(name).ToList();
            if (!processes.Any())
            {
                LogWarning($"Kein Prozess mit Name '{name}' gefunden.");
                return false;
            }

            var process = processes.First();
            return await process.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stoppt einen Prozess asynchron.
        /// </summary>
        public async Task<bool> StopAsync(string processId)
        {
            var process = Get(processId);
            if (process == null)
            {
                LogWarning($"Prozess '{processId}' nicht gefunden.");
                return false;
            }

            return await process.StopAsync();
        }

        /// <summary>
        /// Konvertiert einen Namen in eine Prozess-ID und stoppt den Prozess.
        /// </summary>
        public async Task<bool> StopByNameAsync(string name)
        {
            var processes = FindByName(name).ToList();
            if (!processes.Any())
            {
                LogWarning($"Kein Prozess mit Name '{name}' gefunden.");
                return false;
            }

            var process = processes.First();
            return await process.StopAsync();
        }

        /// <summary>
        /// Startet alle registrierten Prozesse.
        /// </summary>
        public async Task<int> StartAllAsync(CancellationToken cancellationToken = default)
        {
            var processes = GetAll().ToList();
            int successCount = 0;

            foreach (var process in processes)
            {
                if (await process.StartAsync(cancellationToken))
                {
                    successCount++;
                }
            }

            return successCount;
        }

        /// <summary>
        /// Stoppt alle registrierten Prozesse.
        /// </summary>
        public async Task<int> StopAllAsync()
        {
            var processes = GetAll().ToList();
            int successCount = 0;

            foreach (var process in processes)
            {
                if (await process.StopAsync())
                {
                    successCount++;
                }
            }

            return successCount;
        }

        /// <summary>
        /// Prüft die Gesundheit aller Prozesse.
        /// </summary>
        public async Task<IDictionary<string, bool?>> CheckAllHealthAsync()
        {
            var processes = GetAll().ToList();
            var result = new Dictionary<string, bool?>();

            foreach (var process in processes)
            {
                try
                {
                    result[process.ProcessId] = await process.CheckHealthAsync();
                }
                catch (Exception ex)
                {
                    LogError($"Health-Check für '{process.Name}' fehlgeschlagen: {ex.Message}");
                    result[process.ProcessId] = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Listet alle Prozesse mit ihren Zuständen auf.
        /// </summary>
        public IEnumerable<(string ProcessId, string Name, SubprocessState State)> ListProcesses()
        {
            lock (_lock)
            {
                return _processes.Values
                    .Select(p => (p.ProcessId, p.Name, p.State))
                    .ToList();
            }
        }

        /// <summary>
        /// Protokolliert eine Information.
        /// </summary>
        private void LogInfo(string message)
        {
            _logger?.LogInfo($"[ProcessController] {message}");
        }

        /// <summary>
        /// Protokolliert eine Warnung.
        /// </summary>
        private void LogWarning(string message)
        {
            _logger?.LogWarning($"[ProcessController] {message}");
        }

        /// <summary>
        /// Protokolliert einen Fehler.
        /// </summary>
        private void LogError(string message)
        {
            _logger?.LogError($"[ProcessController] {message}");
        }
    }
}
