using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Lifecycle
{
    /// <summary>
    /// Definiert eine spezielle Start-Schnittstelle, die ausschließlich beim
    /// allerersten Start einer Komponente ausgeführt wird. Ideal für
    /// Initialisierungsroutinen wie das Erstellen von Standardkonfigurationen,
    /// Datenbankmigrationen oder Setup-Prozesse.
    /// 
    /// IFirstRun erweitert IOnStart und kann daher sowohl synchrone als auch
    /// asynchrone Startlogik enthalten.
    /// </summary>
    public interface IFirstRun : IOnStart
    {
        /// <summary>
        /// Wird nur beim ersten Start des Systems oder der Komponente ausgeführt.
        /// Diese Methode sollte ausschließlich einmalige Setup-Logik enthalten.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        void OnFirstRun(IRuntimeContext context);

        /// <summary>
        /// Wird nur beim ersten Start des Systems oder der Komponente ausgeführt.
        /// Diese Methode sollte ausschließlich einmalige Setup-Logik enthalten
        /// und eignet sich für asynchrone Operationen.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        Task OnFirstRunAsync(IRuntimeContext context);
    }

}
