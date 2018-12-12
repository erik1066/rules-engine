using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Foundation.RulesEngine.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        private const string COMMENT_KEYWORD = "$comment";
	    private const string DESCRIPTION_KEYWORD = "$description";
        private IValidator _engine;

        public ValidationResult Evaluate(object ruleObj, JObject obj)
        {
            CheckRule(obj);
            ValidationResult vr = EvaluateImplementation(ruleObj, obj);
            return vr;
        }

        protected abstract ValidationResult EvaluateImplementation(object ruleObj, JObject obj);

        protected JObject Transform(Dictionary<string, string> map)
        {
            JObject json = new JObject();

            // TODO: Implement this function?

            return json;
        }

        protected List<ValidationResult> Recurse(object subrule, object value) 
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (value is JObject)
            {
                results.Add(Evaluate(subrule, (JObject) value));
            }
            else if (value is JArray) 
            {
                // TODO: Check for arrays
            } 
            else
            {
                throw new InvalidDataException("Can't reapply the rule on the following object: " + value);
            }

            return results;
        }

        protected bool IsBasicType(object value) 
        {
            bool isNumber = (value is int) || (value is double) || (value is float);
            return isNumber || (value is string) || (value is bool);
        }

        protected bool IsArray(object value) {
            return value is JArray;
        }

        protected bool IsArrayOfArray(object value) 
        {
            if (!IsArray(value))
                return false;
            JArray array = (JArray) value;
            if (array.Count == 0)
                return false;
            bool result = true;

            // TODO: Check for arrays
            // foreach (object subItem in array) 
            // {
            //     result = result && IsArray(subItem);
            // }

            return result;
        }

        protected abstract void CheckRule(object ruleObj);

        public abstract string GetKeyword();

        public void SetEngine(IValidator validator)
        {
            this._engine = validator;
        }
    }
}
