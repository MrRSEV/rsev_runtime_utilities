using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RSEV.Utilities.Processes
{
    /// <summary>
    /// Ein Utility zur Cross-Platform-Auflösung von ausführbaren Dateien.
    /// Unterstützt Umgebungsvariablen, mehrere Fallback-Pfade und Windows/Linux-spezifische Auflösung.
    /// </summary>
    public static class ExecutableResolver
    {
        /// <summary>
        /// Löst ein ausführbares Programm auf, indem mehrere Quellen durchsucht werden.
        /// </summary>
        /// <param name="environmentVariable">Name der Umgebungsvariable, die das Executable angeben könnte.</param>
        /// <param name="windowsPath">Der Pfad unter Windows (relativ zum AppContext.BaseDirectory).</param>
        /// <param name="linuxPath">Der Pfad unter Linux (Systemkommando oder absoluter Pfad).</param>
        /// <returns>Der aufgelöste Pfad zum Executable.</returns>
        /// <exception cref="FileNotFoundException">Wenn kein Executable gefunden wird.</exception>
        public static string Resolve(string environmentVariable, string windowsPath, string linuxPath)
        {
            // 1. Prüfe Umgebungsvariable
            if (!string.IsNullOrWhiteSpace(environmentVariable))
            {
                var envValue = Environment.GetEnvironmentVariable(environmentVariable);
                if (!string.IsNullOrWhiteSpace(envValue) && File.Exists(envValue))
                {
                    return envValue;
                }
            }

            // 2. Platform-spezifische Auflösung
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ResolveWindows(windowsPath);
            }

            return ResolveLinux(linuxPath);
        }

        /// <summary>
        /// Löst einen Windows-Pfad mit mehreren Fallback-Kandidaten auf.
        /// </summary>
        private static string ResolveWindows(string configuredPath)
        {
            var candidates = new[]
            {
                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), configuredPath)),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredPath)),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", configuredPath))
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                $"Windows-Executable nicht gefunden. Durchsuchte Pfade: {string.Join(", ", candidates)}");
        }

        /// <summary>
        /// Löst einen Linux-Pfad auf. Nutzt das Executable direkt oder sucht in $PATH.
        /// </summary>
        private static string ResolveLinux(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new ArgumentException("Linux-Pfad darf nicht leer sein.", nameof(executablePath));
            }

            // Wenn der Pfad absolut ist und existiert, verwende ihn direkt
            if (Path.IsPathRooted(executablePath) && File.Exists(executablePath))
            {
                return executablePath;
            }

            // Sonst gehe davon aus, dass es ein System-Kommando ist (z.B. "node", "python")
            // und wird über $PATH aufgelöst
            return executablePath;
        }

        /// <summary>
        /// Stellt sicher, dass eine Linux-Datei ausführbar ist.
        /// </summary>
        /// <param name="executablePath">Der Pfad zur ausführbaren Datei.</param>
        public static void EnsureExecutablePermissions(string executablePath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return; // Nur unter Linux notwendig
            }

            if (!File.Exists(executablePath))
            {
                return;
            }

            try
            {
                var chmod = System.Diagnostics.Process.Start("/bin/chmod", $"+x \"{executablePath}\"");
                chmod?.WaitForExit();
            }
            catch
            {
                // Fehler beim Setzen der Permissions ignorieren
            }
        }
    }
}
