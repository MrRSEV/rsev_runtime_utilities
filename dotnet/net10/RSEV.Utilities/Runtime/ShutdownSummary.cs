using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Runtime
{
    /// <summary>
    /// Enthält eine Zusammenfassung des Shutdown-Prozesses. Diese Klasse
    /// sammelt Informationen darüber, welche Komponenten erfolgreich
    /// heruntergefahren wurden, welche Fehler aufgetreten sind und wie
    /// lange der Shutdown gedauert hat.
    /// </summary>
    public class ShutdownSummary
    {
        /// <summary>
        /// Zeitpunkt, an dem der Shutdown begonnen hat.
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Zeitpunkt, an dem der Shutdown abgeschlossen wurde.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Liste aller Komponenten, die erfolgreich heruntergefahren wurden.
        /// </summary>
        public List<string> SuccessfulComponents { get; }

        /// <summary>
        /// Liste aller Fehler, die während des Shutdowns aufgetreten sind.
        /// </summary>
        public List<string> Errors { get; }

        /// <summary>
        /// Erstellt eine neue Instanz der ShutdownSummary-Klasse.
        /// </summary>
        public ShutdownSummary()
        {
            StartTime = DateTime.UtcNow;
            SuccessfulComponents = new List<string>();
            Errors = new List<string>();
        }

        /// <summary>
        /// Markiert eine Komponente als erfolgreich heruntergefahren.
        /// </summary>
        /// <param name="componentName">Der Name der Komponente.</param>
        public void AddSuccess(string componentName)
        {
            SuccessfulComponents.Add(componentName);
        }

        /// <summary>
        /// Fügt einen Fehler zur Zusammenfassung hinzu.
        /// </summary>
        /// <param name="error">Die Fehlermeldung.</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }

        /// <summary>
        /// Markiert den Shutdown als abgeschlossen.
        /// </summary>
        public void Complete()
        {
            EndTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gibt eine menschenlesbare Zusammenfassung des Shutdown-Prozesses zurück.
        /// </summary>
        /// <returns>Eine formatierte Zusammenfassung.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Shutdown Summary");
            sb.AppendLine("----------------");
            sb.AppendLine($"Start: {StartTime:u}");
            sb.AppendLine($"End:   {EndTime:u}");
            sb.AppendLine($"Duration: {(EndTime - StartTime).TotalSeconds:F2} seconds");
            sb.AppendLine();

            sb.AppendLine("Successful Components:");
            foreach (var s in SuccessfulComponents)
                sb.AppendLine($"  ✔ {s}");

            sb.AppendLine();

            if (Errors.Count > 0)
            {
                sb.AppendLine("Errors:");
                foreach (var e in Errors)
                    sb.AppendLine($"  ✖ {e}");
            }
            else
            {
                sb.AppendLine("No errors occurred.");
            }

            return sb.ToString();
        }

    }

}
