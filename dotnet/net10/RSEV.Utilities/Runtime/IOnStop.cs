using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Lifecycle
{
    /// <summary>
    /// Definiert eine universelle Stop-Schnittstelle, die von beliebigen
    /// Komponenten implementiert werden kann, um beim Systemstopp oder
    /// Shutdown ausgeführt zu werden. Ideal für Controller, Plugins,
    /// Services oder Module.
    /// </summary>
    public interface IOnStop
    {
        /// <summary>
        /// Wird aufgerufen, wenn das System gestoppt wird und die Komponente
        /// ihre Aufräum- oder Speicherlogik ausführen soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnStop(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn das System gestoppt wird und die
        /// Komponente ihre Aufräum- oder Speicherlogik ausführen soll.
        /// Ideal für I/O-Operationen oder komplexe Shutdown-Prozesse.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnStopAsync(IRuntimeContext context);
    }

}