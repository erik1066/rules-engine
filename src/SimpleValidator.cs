using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Foundation.RulesEngine.Commands;

namespace Foundation.RulesEngine
{
    public class SimpleValidator : IValidator
    {
        private List<Rule> _businessRules = new List<Rule>();
        private Dictionary<string, ICommand> _library = new Dictionary<string, ICommand>();

        public SimpleValidator(Stream rules)
        {
            string rulesStr = string.Empty;
            using (StreamReader reader = new StreamReader(rules, Encoding.UTF8))
            {
                rulesStr = reader.ReadToEnd();
            }
            JObject rulesJson = JObject.Parse(rulesStr);
            Initialize(rulesJson);
        }

        public SimpleValidator(JObject rules)
        {
            Initialize(rules);            
        }

        private void Initialize(JObject rules)
        {
            LoadLibrary();

            List<String> commands = rules.Properties().Select(p => p.Name).ToList();
            
            foreach (string command in commands)
            {
                if (_library.ContainsKey(command)) 
                {
                    _businessRules.Add(Rule.Build(_library[command], rules.GetValue(command)));
                } 
                else
                {
                    throw new InvalidOperationException("The command " + command + " doesn't exist.");
                }
            }
        }

        public ValidationResult Validate(Stream rules)
        {
            string rulesStr = string.Empty;
            using (StreamReader reader = new StreamReader(rules, Encoding.UTF8))
            {
                rulesStr = reader.ReadToEnd();
            }
            JObject rulesJson = JObject.Parse(rulesStr);
            return Validate(rulesJson);
        }

        public ValidationResult Validate(JObject rules)
        {
            var cvr = new CompoundValidationResult();
            cvr.Object = rules.ToString();
            
            foreach (Rule rule in _businessRules) 
            {
                ICommand cmd = rule.Command;
                cmd.SetEngine(this);
                cvr.Add(cmd.Evaluate(rule.Parameters, rules));
            }

            return cvr;
        }

        public ICommand GetCommand(string keyword)
        {
            var success = _library.TryGetValue(keyword, out ICommand command);
            if (success)
            {
                return command;
            }
            else
            {
                throw new InvalidOperationException($"Command {keyword} doesn't exist");
            }
        }

        private void LoadLibrary() 
        {
            var type = typeof(ICommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Where(p => p.IsAbstract == false);
            
            foreach (var iType in types)
            {
                var command = (ICommand)Activator.CreateInstance(iType);
                _library.Add(command.GetKeyword(), command);
            }
        }
    }
}
