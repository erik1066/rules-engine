using System.IO;
using System.Text;

using Newtonsoft.Json.Linq;

using Foundation.RulesEngine.Commands;

namespace Foundation.RulesEngine
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Rule { get; set; }
        public string Object { get; set; }
        public string Command { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public static ValidationResult Build(bool isValid, string rule, string obj, ICommand command) 
        {
            ValidationResult result = new ValidationResult();
            result.Command = command.GetKeyword();
            result.Object = obj;
            if (command is SingleCommand) 
            {
                SingleCommand sc = (SingleCommand) command;
                result.Description = sc.Description;
                result.Comment = sc.Comment;
            }
            result.IsValid = isValid;
            result.Rule = rule;
            return result;
	    }

        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rule: ");
            sb.Append(Rule);
            sb.Append("\nObject: ");
            sb.Append(Object);
            sb.Append("\nCommand: ");
            sb.Append(Command);
            sb.Append("\nComment: ");
            sb.Append(Comment);
            sb.Append("\nDescription: ");
            sb.Append(Description);
            sb.Append("\nIs Valid? ");
            sb.Append(IsValid);
            return sb.ToString();
	    }

        public virtual string ToString(int index) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rule: ".PadLeft(index * 2, ' '));
            sb.Append(Rule);
            sb.Append("\n");
            sb.Append("Object: ".PadLeft(index * 2, ' '));
            sb.Append(Object);
            sb.Append("\n");
            sb.Append("Command: ".PadLeft(index * 2, ' '));
            sb.Append(Command);
            sb.Append("\n");
            sb.Append("Comment: ".PadLeft(index * 2, ' '));
            sb.Append(Comment);
            sb.Append("\n");
            sb.Append("Description: ".PadLeft(index * 2, ' '));
            sb.Append(Description);
            sb.Append("\n");
            sb.Append("Is Valid? ".PadLeft(index * 2, ' '));
            sb.Append(IsValid);
            sb.Append("\n");
            sb.Append("---------------------".PadLeft(index * 2, ' '));
            return sb.ToString();
        }
    }
}
