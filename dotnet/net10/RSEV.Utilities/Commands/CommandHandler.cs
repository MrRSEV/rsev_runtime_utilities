using System;
using System.Collections.Generic;
using System.Text;
using RSEV.Utilities.Runtime;

namespace RSEV.Utilities.Commands
{
    public class CommandHandler
    {
        protected readonly Dictionary<string, ICommand> _commands
            = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registriert einen Command im Handler.
        /// </summary>
        public virtual void Register(ICommand command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Name))
                return;

            _commands[command.Name] = command;
        }

        /// <summary>
        /// Registriert mehrere Commands auf einmal.
        /// </summary>
        public virtual void RegisterRange(IEnumerable<ICommand> commands)
        {
            foreach (var cmd in commands)
                Register(cmd);
        }

        /// <summary>
        /// Prüft, ob ein Command existiert.
        /// </summary>
        public virtual bool Exists(string name)
        {
            return _commands.ContainsKey(name);
        }

        /// <summary>
        /// Holt einen Command anhand seines Namens.
        /// </summary>
        public virtual ICommand Get(string name)
        {
            _commands.TryGetValue(name, out var cmd);
            return cmd;
        }

        /// <summary>
        /// Führt einen Command aus, wenn er existiert.
        /// </summary>
        public virtual async Task<bool> ExecuteAsync(
            string name,
            IRuntimeContext context,
            params string[] arguments)
        {
            if (!_commands.TryGetValue(name, out var cmd))
                return false;

            await cmd.ExecuteAsync(context, arguments);
            return true;
        }

        /// <summary>
        /// Gibt alle registrierten Commands zurück.
        /// </summary>
        public virtual IEnumerable<ICommand> GetAll()
        {
            return _commands.Values;
        }
    }
}
