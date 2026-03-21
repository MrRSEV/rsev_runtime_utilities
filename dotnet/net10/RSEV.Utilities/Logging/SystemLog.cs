using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace RSEV.Utilities.Logging
{
    /// <summary>
    /// Eine erweiterbare Logging-Klasse mit Logrotation, farbiger Konsolenausgabe
    /// und verschiedenen Log-Leveln. Diese Klasse ist nicht statisch, damit sie
    /// in Projekten überschrieben oder erweitert werden kann.
    /// </summary>
    public class SystemLog : ILogger
    {
        protected StreamWriter? _logWriter;
        protected string _logDirectory = "";
        protected string _logFilePath = "";
        protected readonly string _logFileName = "latest.log";
        protected bool _isClosed = false;

        private readonly object _lock = new object();

        /// <summary>
        /// Optionales benutzerdefiniertes Log-Verzeichnis.
        /// Wenn null, wird automatisch ein OS-abhängiges Standardverzeichnis verwendet.
        /// </summary>
        protected readonly string? _customLogDirectory;

        /// <summary>
        /// Unterstützte Log-Level.
        /// </summary>
        public enum LogLevel
        {
            Debug = 0,
            Info = 1,
            Warnung = 2,
            Error = 3,
            Kritisch = 4
        }

        /// <summary>
        /// Farben für die Konsolenausgabe.
        /// </summary>
        public enum LogColor
        {
            Gray,
            Green,
            Yellow,
            Red,
            Cyan,
            Magenta,
            White
        }

        protected virtual string[] LogLevelNames { get; } =
        {
            "DEBUG",
            "INFO",
            "WARNUNG",
            "ERROR",
            "KRITISCH"
        };


        /// <summary>
        /// Erstellt eine neue Instanz des SystemLog.
        /// Optional kann ein eigenes Log-Verzeichnis angegeben werden.
        /// </summary>
        public SystemLog(string? customLogDirectory = null)
        {
            _customLogDirectory = customLogDirectory;
        }


        /// <summary>
        /// Initialisiert das Logging-System und führt Logrotation durch.
        /// </summary>
        public virtual void Init()
        {
            if (!string.IsNullOrWhiteSpace(_customLogDirectory))
            {
                // Benutzerdefiniertes Verzeichnis hat Vorrang
                _logDirectory = _customLogDirectory!;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows: ./log
                _logDirectory = Path.Combine(AppContext.BaseDirectory, "log");
            }
            else
            {
                // Linux: /var/log/<BaseDirectoryName>
                string baseDir = new DirectoryInfo(AppContext.BaseDirectory).Name;
                _logDirectory = Path.Combine("/var/log", baseDir);
            }

            Directory.CreateDirectory(_logDirectory);
            _logFilePath = Path.Combine(_logDirectory, _logFileName);

            RotateIfExists();

            _logWriter = new StreamWriter(_logFilePath, append: false)
            {
                AutoFlush = true
            };
        }



        /// <summary>
        /// Führt eine Logrotation durch, falls bereits eine Logdatei existiert.
        /// </summary>
        protected virtual void RotateIfExists()
        {
            if (!File.Exists(_logFilePath))
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string archivedPath = Path.Combine(_logDirectory, $"{timestamp}.log");

            File.Move(_logFilePath, archivedPath);
        }

        // ---------------------------------------------------------
        //  Convenience-Methoden
        // ---------------------------------------------------------

        public virtual void LogDebug(string message) =>
            Log(LogLevel.Debug, message);

        public virtual void LogInfo(string message) =>
            Log(LogLevel.Info, message);

        public virtual void LogWarning(string message) =>
            Log(LogLevel.Warnung, message);

        public virtual void LogError(string message) =>
            Log(LogLevel.Error, message);

        public virtual void LogCritical(string message) =>
            Log(LogLevel.Kritisch, message);

        public virtual void Log(string source, string message, LogLevel level) =>
            Log(level, $"[{source}] {message}");

        public virtual void Log(LogLevel level, string message) =>
            Log(level, message, null);


        // ---------------------------------------------------------
        //  Haupt-Log-Methode
        // ---------------------------------------------------------

        public virtual void Log(LogLevel level, string message, LogColor? overrideColor = null)
        {
            if (_isClosed)
                return;

            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("dd-MM HH:mm:ss");
                string levelName = LogLevelNames[(int)level];
                string formatted = $"[{timestamp}] - [{levelName}] -> {message}";

                ConsoleColor? color = overrideColor.HasValue
                    ? ToConsoleColor(overrideColor.Value)
                    : GetDefaultColorForLevel(level);

                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                    Console.WriteLine(formatted);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(formatted);
                }

                try
                {
                    _logWriter?.WriteLine(formatted);
                }
                catch (ObjectDisposedException)
                {
                    // Ignorieren
                }
            }
        }

        // ---------------------------------------------------------
        //  Shutdown
        // ---------------------------------------------------------

        public virtual void Close()
        {
            if (_isClosed)
                return;

            lock (_lock)
            {
                _logWriter?.Close();
                _isClosed = true;

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string archivedPath = Path.Combine(_logDirectory, $"{timestamp}.log");

                try
                {
                    File.Move(_logFilePath, archivedPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNUNG] Konnte Log-Datei nicht archivieren: {ex.Message}");
                }
            }
        }

        // ---------------------------------------------------------
        //  Hilfsmethoden
        // ---------------------------------------------------------

        protected virtual ConsoleColor? GetDefaultColorForLevel(LogLevel level) => level switch
        {
            LogLevel.Warnung => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Magenta,
            LogLevel.Kritisch => ConsoleColor.Red,
            LogLevel.Debug => ConsoleColor.Cyan,
            _ => null
        };

        protected virtual ConsoleColor ToConsoleColor(LogColor color) => color switch
        {
            LogColor.Gray => ConsoleColor.Gray,
            LogColor.Green => ConsoleColor.Green,
            LogColor.Yellow => ConsoleColor.Yellow,
            LogColor.Red => ConsoleColor.Red,
            LogColor.Cyan => ConsoleColor.Cyan,
            LogColor.Magenta => ConsoleColor.Magenta,
            LogColor.White => ConsoleColor.White,
            _ => ConsoleColor.Gray
        };

    }

}
