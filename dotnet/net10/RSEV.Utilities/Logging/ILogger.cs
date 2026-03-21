using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Logging
{
    /// <summary>
    /// Definiert eine universelle Logging-Schnittstelle, die von beliebigen
    /// Logger-Implementierungen genutzt werden kann. Ideal für FileLogger,
    /// ConsoleLogger, RemoteLogger oder zusammengesetzte Logger.
    /// </summary>
    public interface ILogger
    {
        void Log(SystemLog.LogLevel level, string message);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogCritical(string message);
    }

}
