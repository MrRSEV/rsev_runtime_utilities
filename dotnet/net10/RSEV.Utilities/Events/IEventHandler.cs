using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Definiert ein generisches Event-Handler-Interface. Implementierungen
    /// können damit Ereignisse synchron oder asynchron verarbeiten.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public interface IEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Verarbeitet ein Ereignis synchron.
        /// </summary>
        /// <param name="sender">Der Absender des Ereignisses.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        void Handle(object sender, TEventArgs args);

        /// <summary>
        /// Verarbeitet ein Ereignis asynchron.
        /// </summary>
        /// <param name="sender">Der Absender des Ereignisses.</param>
        /// <param name="args">Die Ereignisargumente.</param>
        Task HandleAsync(object sender, TEventArgs args);
    }

}
