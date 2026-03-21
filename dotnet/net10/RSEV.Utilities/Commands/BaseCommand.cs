using RSEV.Utilities.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace RSEV.Utilities.Commands
{
    public class BaseCommand : ICommand
    {
        public virtual string Name { get; }
        public virtual string Description { get; }

        public BaseCommand(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public virtual Task ExecuteAsync(IRuntimeContext context, params string[] arguments)
        {
            return Task.CompletedTask;
        }
    }
}
