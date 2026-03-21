using System;
using System.Collections.Generic;
using System.Text;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Controllers
{
    /// <summary>
    /// Definiert die grundlegende Struktur eines Controllers. Ein Controller
    /// repräsentiert eine logische Einheit, die bestimmte Aufgaben übernimmt,
    /// z. B. Initialisierung, Verarbeitung, Verwaltung oder Steuerung von
    /// Subsystemen. Dieses Interface ist universell und kann für Hauptcontroller,
    /// Subcontroller, Plugin-Controller oder beliebige andere Controllerformen
    /// verwendet werden.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Der eindeutige Name des Controllers. Wird zur Identifikation,
        /// Registrierung und Verwaltung verwendet.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Wird aufgerufen, wenn der Controller initialisiert werden soll.
        /// Hier können Ressourcen geladen, Abhängigkeiten aufgebaut oder
        /// interne Strukturen vorbereitet werden.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void Initialize(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Controller initialisiert werden soll.
        /// Ideal für I/O-Operationen oder komplexe Startprozesse.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task InitializeAsync(IRuntimeContext context);

        /// <summary>
        /// Wird aufgerufen, wenn der Controller gestoppt oder heruntergefahren wird.
        /// Hier können Ressourcen freigegeben, Verbindungen geschlossen oder
        /// Zustände gespeichert werden.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void Shutdown(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Controller gestoppt oder
        /// heruntergefahren wird. Ideal für I/O-Operationen oder komplexe
        /// Speicherprozesse.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task ShutdownAsync(IRuntimeContext context);
    }

}
