// using System;
// using System.IO;
// using System.Text;
// using Newtonsoft.Json.Linq;

// namespace Foundation.RulesEngine.Commands
// {
//     public sealed class EqualsCommand : SingleCommand
//     {
//         public override string GetKeyword() => "$eq";

//         protected override void IsValid(JObject rule, string jsonPath, object value, bool found, CompoundValidationResult result) 
//         {
//             var ruleValue = rule.GetValue(jsonPath);

//             if (ruleValue == null)
//             {
//                 if (value == null && found)
//                 {
//                     result.Add(ValidationResult.Build(true, rule.ToString(), null, this));
//                 }
//                 else
//                 {
//                     result.Add(ValidationResult.Build(false, rule.ToString(), value != null ? value.ToString() : null, this));
//                 }
//             }
//             else if (value != null && IsBasicType(value)) 
//             {
//                 var ruleValueStr = ruleValue.ToString();
//                 bool valid = value.Equals(ruleValueStr);
//                 result.Add(ValidationResult.Build(valid, rule.ToString(), value.ToString(), this));
//             }
//             else if (ruleValue is JToken)
//             {
//                 result.AddMany(Recurse(ruleValue, value));
//             }
//             // TODO: Add other checks, e.g. for arrays
//             else
//             {
//                 throw new InvalidOperationException("Don't understand the parameter");
//             }
//         }
//     }
// }
