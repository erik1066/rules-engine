using System.IO;
using Newtonsoft.Json.Linq;
using Foundation.RulesEngine.Commands;

namespace Foundation.RulesEngine
{
    public class Rule
    {
        public ICommand Command { get; }
        public object Parameters { get; }

        private Rule(ICommand command, object parameters)
        {
            Command = command;
            Parameters = parameters;
        }

        public static Rule Build(ICommand command, object parameters) => new Rule(command, parameters);
    }
}
