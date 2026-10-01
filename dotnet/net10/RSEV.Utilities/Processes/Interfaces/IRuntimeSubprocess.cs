using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Definiert einen verwalteten Subprozess mit vollständigem Lifecycle und Event-Handling.
    /// </summary>
    public interface IRuntimeSubprocess
    {
        /// <summary>
        /// Die eindeutige ID des Prozesses.
        /// </summary>
        string ProcessId { get; }

        /// <summary>
        /// Der Name des Prozesses.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Das ausführbare Programm (Pfad oder Kommando).
        /// </summary>
        string Executable { get; }

        /// <summary>
        /// Die Kommandozeilen-Argumente für den Prozess.
        /// </summary>
        string[] Arguments { get; }

        /// <summary>
        /// Der aktuuelle Zustand des Prozesses.
        /// </summary>
        SubprocessState State { get; }

        /// <summary>
        /// Der Exit-Code des Prozesses (null, wenn noch laufend).
        /// </summary>
        int? ExitCode { get; }

        /// <summary>
        /// Ein Key-Value-Speicher für prozesstyp-spezifische Eigenschaften.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Event, das ausgelöst wird, wenn der Prozess auf stdout schreibt.
        /// </summary>
        event Action<IRuntimeSubprocess, string> OnStdOut;

        /// <summary>
        /// Event, das ausgelöst wird, wenn der Prozess auf stderr schreibt.
        /// </summary>
        event Action<IRuntimeSubprocess, string> OnStdErr;

        /// <summary>
        /// Startet den Prozess asynchron.
        /// </summary>
        /// <param name="cancellationToken">Token zur Abbruchverwaltung.</param>
        /// <returns>True wenn erfolgreich gestartet, sonst false.</returns>
        Task<bool> StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stoppt den Prozess asynchron.
        /// </summary>
        /// <returns>True wenn erfolgreich beendet, sonst false.</returns>
        Task<bool> StopAsync();

        /// <summary>
        /// Startet den Prozess neu asynchron.
        /// </summary>
        /// <param name="cancellationToken">Token zur Abbruchverwaltung.</param>
        /// <returns>True wenn erfolgreich neugestartet, sonst false.</returns>
        Task<bool> RestartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Prüft die Gesundheit des Prozesses asynchron (optional prozesstyp-spezifisch).
        /// </summary>
        /// <returns>True wenn der Prozess ist gesund, null wenn nicht implementiert, false wenn unhealthy.</returns>
        Task<bool?> CheckHealthAsync();
    }
}
