using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSEV.Utilities.Runtime
{
    /// <summary>
    /// Behandelt unerwartete Fehler oder Abstürze des Systems und führt einen
    /// kontrollierten Shutdown durch. Der Handler deaktiviert Plugins,
    /// setzt den RuntimeContext in einen konsistenten Zustand und erzeugt
    /// eine vollständige <see cref="ShutdownSummary"/>.
    ///
    /// Hinweis:
    /// Komponenten, die zusätzliche Aufräumlogik benötigen, sollten das
    /// Interface <see cref="RSEV.Utilities.Lifecycle.IOnStop"/> implementieren
    /// und insbesondere die Methode <see cref="RSEV.Utilities.Lifecycle.IOnStop.OnStopAsync"/>
    /// bereitstellen. Es wird empfohlen, innerhalb des Interfaces eigene
    /// OnStop-Klassen oder -Handler zu definieren und diese im
    /// UnexpectedExitHandler gezielt anzusteuern, um einen vollständigen
    /// und sauberen Shutdown sicherzustellen.
    /// </summary>
    public class UnexpectedExitHandler
    {
        /// <summary>
        /// Der aktuelle Runtime-Kontext.
        /// </summary>
        protected readonly IRuntimeContext _context;

        /// <summary>
        /// Erstellt eine neue Instanz des UnexpectedExitHandler.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        public UnexpectedExitHandler(IRuntimeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein unerwarteter Fehler auftritt.
        /// Führt einen kontrollierten Shutdown durch und erzeugt
        /// eine ShutdownSummary.
        /// </summary>
        /// <param name="exception">Die aufgetretene Ausnahme.</param>
        /// <returns>Eine vollständige ShutdownSummary.</returns>
        public virtual ShutdownSummary Handle(Exception exception)
        {
            var summary = new ShutdownSummary();
            summary.AddError($"Unexpected exception: {exception}");

            try
            {
                _context.IsRunning = false;
            }
            catch (Exception ex)
            {
                summary.AddError($"Failed to update IsRunning flag: {ex}");
            }

            try
            {
                _context.PluginRegistry.DisableAll(_context);
                summary.AddSuccess("PluginRegistry");
            }
            catch (Exception ex)
            {
                summary.AddError($"PluginRegistry shutdown failed: {ex}");
            }

            summary.Complete();
            return summary;
        }

        /// <summary>
        /// Asynchrone Version des Unexpected-Exit-Handlers. Führt einen
        /// kontrollierten Shutdown durch und erzeugt eine vollständige
        /// <see cref="ShutdownSummary"/>.
        ///
        /// Hinweis:
        /// Für Komponenten, die zusätzliche Aufräumlogik benötigen, sollte
        /// <see cref="RSEV.Utilities.Lifecycle.IOnStop.OnStopAsync"/> implementiert
        /// und im Rahmen des Shutdown-Prozesses aufgerufen werden.
        /// </summary>
        public virtual async Task<ShutdownSummary> HandleAsync(Exception exception)
        {
            var summary = new ShutdownSummary();
            summary.AddError($"Unexpected exception: {exception}");

            try
            {
                _context.IsRunning = false;
            }
            catch (Exception ex)
            {
                summary.AddError($"Failed to update IsRunning flag: {ex}");
            }

            try
            {
                await _context.PluginRegistry.DisableAllAsync(_context);
                summary.AddSuccess("PluginRegistry");
            }
            catch (Exception ex)
            {
                summary.AddError($"PluginRegistry shutdown failed: {ex}");
            }

            summary.Complete();
            return summary;
        }

    }

}