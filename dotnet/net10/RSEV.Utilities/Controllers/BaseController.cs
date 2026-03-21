using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Controllers
{
    /// <summary>
    /// Eine universelle, erweiterbare Basisklasse für Controller. Sie stellt
    /// Standardimplementierungen für Initialisierung und Shutdown bereit und
    /// dient als Grundlage für Hauptcontroller, Subcontroller, Plugin-Controller
    /// oder beliebige andere Controllerformen.
    /// </summary>
    public abstract class BaseController : IController
    {
        /// <summary>
        /// Der eindeutige Name des Controllers. Wird zur Identifikation,
        /// Registrierung und Verwaltung verwendet.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Erstellt eine neue Instanz des BaseController.
        /// </summary>
        /// <param name="name">Der eindeutige Name des Controllers.</param>
        protected BaseController(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Wird aufgerufen, wenn der Controller initialisiert werden soll.
        /// Diese Methode ruft intern OnInitialize auf, welches von
        /// abgeleiteten Klassen überschrieben werden kann.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void Initialize(IRuntimeContext context)
        {
            OnInitialize(context);
        }

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Controller initialisiert werden soll.
        /// Diese Methode ruft intern OnInitializeAsync auf, welches von
        /// abgeleiteten Klassen überschrieben werden kann.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task InitializeAsync(IRuntimeContext context)
        {
            await OnInitializeAsync(context);
        }

        /// <summary>
        /// Wird aufgerufen, wenn der Controller heruntergefahren wird.
        /// Diese Methode ruft intern OnShutdown auf, welches von
        /// abgeleiteten Klassen überschrieben werden kann.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual void Shutdown(IRuntimeContext context)
        {
            OnShutdown(context);
        }

        /// <summary>
        /// Wird asynchron aufgerufen, wenn der Controller heruntergefahren wird.
        /// Diese Methode ruft intern OnShutdownAsync auf, welches von
        /// abgeleiteten Klassen überschrieben werden kann.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public virtual async Task ShutdownAsync(IRuntimeContext context)
        {
            await OnShutdownAsync(context);
        }

        /// <summary>
        /// Hook-Methode für die Initialisierung. Kann von abgeleiteten
        /// Klassen überschrieben werden, um eigene Startlogik zu implementieren.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        protected virtual void OnInitialize(IRuntimeContext context)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Asynchroner Hook für die Initialisierung. Kann von abgeleiteten
        /// Klassen überschrieben werden, um eigene Startlogik zu implementieren.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        protected virtual Task OnInitializeAsync(IRuntimeContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Hook-Methode für das Herunterfahren. Kann von abgeleiteten
        /// Klassen überschrieben werden, um eigene Shutdown-Logik zu implementieren.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        protected virtual void OnShutdown(IRuntimeContext context)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Asynchroner Hook für das Herunterfahren. Kann von abgeleiteten
        /// Klassen überschrieben werden, um eigene Shutdown-Logik zu implementieren.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        protected virtual Task OnShutdownAsync(IRuntimeContext context)
        {
            return Task.CompletedTask;
        }

    }

}
