using System.IO;
using Newtonsoft.Json.Linq;
using Foundation.RulesEngine.Commands;

namespace Foundation.RulesEngine
{
    public interface IValidator
    {
        ValidationResult Validate(Stream stream);

        ValidationResult Validate(JObject rules);

        ICommand GetCommand(string keyword);
    }
}
