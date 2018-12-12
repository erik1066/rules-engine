using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Foundation.RulesEngine.Commands
{
    public interface ICommand
    {
        ValidationResult Evaluate(object rule, JObject obj);

        string GetKeyword();

        void SetEngine(IValidator validator);
    }
}
