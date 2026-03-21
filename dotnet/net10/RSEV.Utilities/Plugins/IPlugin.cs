using System;
using System.Collections.Generic;
using System.Text;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Plugins
{
    /// <summary>
    /// Definiert die grundlegende Struktur eines Plugins. Plugins können
    /// dynamisch geladen, aktiviert, deaktiviert und neu geladen werden.
    /// Dieses Interface dient als universelle Vorlage für alle Plugin-Typen.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Der eindeutige Name des Plugins.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Eine kurze Beschreibung des Plugins.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Wird aufgerufen, wenn das Plugin aktiviert wird.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnEnable(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn das Plugin aktiviert wird.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnEnableAsync(IRuntimeContext context);

        /// <summary>
        /// Wird aufgerufen, wenn das Plugin deaktiviert wird.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnDisable(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn das Plugin deaktiviert wird.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnDisableAsync(IRuntimeContext context);

        /// <summary>
        /// Wird aufgerufen, wenn das Plugin neu geladen werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnReload(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn das Plugin neu geladen werden soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnReloadAsync(IRuntimeContext context);
    }

}
