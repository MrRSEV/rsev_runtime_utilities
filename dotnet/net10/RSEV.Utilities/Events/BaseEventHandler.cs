using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Abstrakte Basisklasse für Event-Handler. Sie bietet Standard-Implementierungen
    /// für synchrone und asynchrone Aufrufe und kann von konkreten Handlern erweitert werden.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public abstract class BaseEventHandler<TEventArgs> : IEventHandler<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Verarbeitet das Ereignis synchron. Ruft intern OnHandle auf.
        /// </summary>
        public virtual void Handle(object sender, TEventArgs args)
        {
            OnHandle(sender, args);
        }

        /// <summary>
        /// Verarbeitet das Ereignis asynchron. Ruft intern OnHandleAsync auf.
        /// </summary>
        public virtual Task HandleAsync(object sender, TEventArgs args)
        {
            return OnHandleAsync(sender, args);
        }

        /// <summary>
        /// Hook-Methode für die synchrone Verarbeitung. Kann überschrieben werden.
        /// </summary>
        protected virtual void OnHandle(object sender, TEventArgs args)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Hook-Methode für die asynchrone Verarbeitung. Kann überschrieben werden.
        /// </summary>
        protected virtual Task OnHandleAsync(object sender, TEventArgs args)
        {
            return Task.CompletedTask;
        }
    }

}
