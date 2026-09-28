using System;
using System.Threading.Tasks;

namespace RSEV.Utilities.Events
{
    /// <summary>
    /// Abstrakte Basisklasse für Event-Listener. Sie bietet Standard-Implementierungen
    /// für synchrone und asynchrone Reaktionen auf Ereignisse.
    /// </summary>
    /// <typeparam name="TEventArgs">Der Typ der Ereignisargumente (erbt von EventArgs).</typeparam>
    public abstract class BaseEventListener<TEventArgs> : IEventListener<TEventArgs>
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Reagiert synchron auf ein Ereignis. Ruft intern OnListen auf.
        /// </summary>
        public virtual void Listen(object sender, TEventArgs args)
        {
            OnListen(sender, args);
        }

        /// <summary>
        /// Reagiert asynchron auf ein Ereignis. Ruft intern OnListenAsync auf.
        /// </summary>
        public virtual Task ListenAsync(object sender, TEventArgs args)
        {
            return OnListenAsync(sender, args);
        }

        /// <summary>
        /// Hook-Methode für die synchrone Reaktion. Kann überschrieben werden.
        /// </summary>
        protected virtual void OnListen(object sender, TEventArgs args)
        {
            // Standard: nichts tun
        }

        /// <summary>
        /// Hook-Methode für die asynchrone Reaktion. Kann überschrieben werden.
        /// </summary>
        protected virtual Task OnListenAsync(object sender, TEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
