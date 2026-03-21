using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Lifecycle
{
    /// <summary>
    /// Definiert eine universelle Start-Schnittstelle, die von beliebigen
    /// Komponenten implementiert werden kann, um beim Systemstart ausgeführt
    /// zu werden. Ideal für Controller, Plugins, Services oder Module.
    /// </summary>
    public interface IOnStart
    {
        /// <summary>
        /// Wird aufgerufen, wenn das System startet und die Komponente
        /// ihre Startlogik ausführen soll.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnStart(IRuntimeContext context);

        /// <summary>
        /// Wird asynchron aufgerufen, wenn das System startet und die Komponente
        /// ihre Startlogik ausführen soll. Ideal für I/O-Operationen oder
        /// komplexe Initialisierungsprozesse.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnStartAsync(IRuntimeContext context);
    }

}
