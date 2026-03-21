using RSEV.Utilities.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Der eindeutige Name des Commands.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Eine kurze Beschreibung, die erklärt, was der Command macht.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Führt den Command aus.
        /// </summary>
        /// <param name="context">Der aktuelle Runtime-Kontext.</param>
        /// <param name="arguments">Optionale Argumente für den Command.</param>
        Task ExecuteAsync(IRuntimeContext context, params string[] arguments);
    }
}
