using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Foundation.RulesEngine.Commands
{
    public abstract class SingleCommand : AbstractCommand
    {
        private const string COMMENT_KEYWORD = "$comment";
	    private const string DESCRIPTION_KEYWORD = "$description";
        private IValidator _engine;

        public string Comment { get; private set; }
        public string Description { get; private set; }

        protected override void CheckRule(object rule) 
        {
		    if (!(rule is JObject))
            {
			    throw new InvalidOperationException("The rule parameter has to be a JSON Object.");
            }
	    }

        protected override ValidationResult EvaluateImplementation(object ruleObj, JObject obj)
        {
            JObject rule = (JObject) ruleObj;

            List<String> keys = rule.Properties().Select(p => p.Name).ToList();

            if (keys.Contains(COMMENT_KEYWORD))
            {
                Comment = rule.GetValue(COMMENT_KEYWORD).ToString();
            }
            if (keys.Contains(DESCRIPTION_KEYWORD))
            {
                Description = rule.GetValue(DESCRIPTION_KEYWORD).ToString();
            }
            
            var result = new CompoundValidationResult(ruleObj.ToString(), obj != null ? obj.ToString() : null, this);

            foreach (string key in keys)
            {
                if (!key.Equals(COMMENT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !key.Equals(DESCRIPTION_KEYWORD, StringComparison.OrdinalIgnoreCase))
                {
                    CheckJsonPath(key, obj, rule, result);
                }
            }

            return result;
        }

        private void CheckJsonPath(string key, JObject obj, JObject rule, CompoundValidationResult result)
        {
            object value = null;
            bool found = true;

            try
            {
                if (obj != null)
                {
                    value = obj.SelectToken(key).ToString();
                }
                else
                {
                    found = false;
                }
            }
            catch
            {
                found = false;
            }

            IsValid(Purify(rule, key), key, value, found, result);
        }

        protected abstract void IsValid(JObject rule, string jsonPath, object value, bool found, CompoundValidationResult result);

        private JObject Purify(JObject rule, String key) 
        {
            var value = rule.GetValue(key).ToString();

            var purifiedVersion = new JObject(
                new JProperty(key, value));
            
            return purifiedVersion;
        }
    }
}