namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Definiert die möglichen Zustände eines verwalteten Subprozesses.
    /// </summary>
    public enum SubprocessState
    {
        /// <summary>
        /// Der Prozess wurde erstellt, aber noch nicht gestartet.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Der Prozess wird gerade gestartet.
        /// </summary>
        Starting = 1,

        /// <summary>
        /// Der Prozess läuft.
        /// </summary>
        Running = 2,

        /// <summary>
        /// Der Prozess wird gerade beendet.
        /// </summary>
        Stopping = 3,

        /// <summary>
        /// Der Prozess wurde beendet.
        /// </summary>
        Stopped = 4,

        /// <summary>
        /// Der Prozess ist fehlgeschlagen oder abgestürzt.
        /// </summary>
        Faulted = 5
    }
}
