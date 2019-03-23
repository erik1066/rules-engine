using Newtonsoft.Json.Linq;
using Foundation.RulesEngine.Models;

namespace Foundation.RulesEngine.Validators
{
    public interface IValidator
    {
        ValidationResults Validate(JObject data, JObject rules);
    }
}
