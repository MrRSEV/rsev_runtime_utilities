using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Definiert ein generisches Event-Listener-Interface. Implementierungen
    /// können Ereignisse empfangen und synchron oder asynchron weiterverarbeiten.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public interface IEventListener<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Reagiert synchron auf ein Ereignis.
        /// </summary>
        /// <param name="sender">Der Absender des Ereignisses.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        void Listen(object sender, TEventArgs args);

        /// <summary>
        /// Reagiert asynchron auf ein Ereignis.
        /// </summary>
        /// <param name="sender">Der Absender des Ereignisses.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        Task ListenAsync(object sender, TEventArgs args);
    }
}
