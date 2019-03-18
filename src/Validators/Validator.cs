using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Foundation.RulesEngine.Models;

namespace Foundation.RulesEngine.Validators
{
    public sealed class Validator
    {
        private static readonly HashSet<string> _supportedCommandNames = new HashSet<string>() { "$all", "$exists", "$eq", "$ne", "$gt", "$gte", "$lt", "$lte", "$in", "$nin", "$regex", "$mod", "$size", "$type" };

        public ValidationResults Validate(JObject data, JObject rules)
        {
            return ValidateRules(data, rules);
        }

        private ValidationResults ValidateRules(JObject data, JObject rules)
        {
            List<string> operators = rules.Properties().Select(p => p.Name).ToList();
            var results = new List<ValidationResult>();

            foreach (var op in operators)
            {
                if (_supportedCommandNames.Contains(op))
                {
                    var rule = rules[op];
                    var commandResult = ValidateRule(op, data, rule);
                    results.AddRange(commandResult);
                }
                else
                {
                    Console.WriteLine($"Skipped processing rule: {op}");
                    // throw new InvalidOperationException($"Command not recognized: {op}");
                }
            }

            var validationResults = new ValidationResults(results);
            return validationResults;
        }

        private List<ValidationResult> ValidateRule(string op, JObject data, JToken rule)
        {
            var results = new List<ValidationResult>();
            var ruleObject = rule is JObject ? ((JObject)rule) : throw new InvalidOperationException("Unrecognized rule definition");

            List<string> jsonPaths = ruleObject.Properties().Select(p => p.Name).ToList();

            foreach (var jsonPath in jsonPaths)
            {
                // get the data at the specified path
                var dataTokens = data.SelectTokens(jsonPath);

                // get the rule operands
                var ruleTokens = ruleObject[jsonPath];

                if (ruleTokens is JValue)
                {
                    var ruleValue = ruleTokens as JValue;
                    var dataToken = dataTokens.FirstOrDefault();

                    if (dataToken is JValue)
                    {
                        var dataValue = dataToken as JValue;
                        var result = ValidateSingleData(op, dataValue, ruleValue);
                        if (result.IsValid == false)
                        {
                            results.Add(result);
                        }
                    }
                    else if (dataToken is JObject)
                    {
                        // var dataObject = dataToken as JObject;
                        var result = ValidateSingleData(op, new JValue("exists"), ruleValue);
                        if (result.IsValid == false)
                        {
                            results.Add(result);
                        }
                    }
                    else if (dataToken is JArray)
                    {

                        JArray ruleValueArray = new JArray();
                        ruleValueArray.Add(ruleValue);

                        var result = ValidateArrayData(op, dataTokens, ruleValueArray);
                        if (result.IsValid == false)
                        {
                            results.Add(result);
                        }

                    }
                    else if (dataToken == null)
                    {
                        var result = ValidateSingleData(op, null, ruleValue);
                        if (result.IsValid == false)
                        {
                            results.Add(result);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unrecognized item for rule {op}");
                    }
                }
                else if (ruleTokens is JArray)
                {
                    var ruleValueArray = ruleTokens as JArray;

                    var result = ValidateArrayData(op, dataTokens, ruleValueArray);
                    if (result.IsValid == false)
                    {
                        results.Add(result);
                    }
                }
                else
                {
                    foreach (var ruleToken in ruleTokens)
                    {
                        var commandResults = ValidateRuleData(op, dataTokens, ruleToken);
                        results.AddRange(commandResults);
                    }
                }
            }

            return results;
        }

        private List<ValidationResult> ValidateRuleData(string op, IEnumerable<JToken> dataTokens, JToken ruleToken)
        {
            var results = new List<ValidationResult>();

            if (ruleToken is JProperty)
            {
                var ruleProperty = ruleToken as JProperty;

                string rulePropertyName = ruleProperty.Name;

                foreach (var dataToken in dataTokens)
                {
                    var dataObject = ((JObject)dataToken);

                    foreach (var dataObjectToken in dataObject.Children())
                    {
                        var dataObjectProperty = ((JProperty)dataObjectToken);

                        if (dataObjectProperty.Name.Equals(ruleProperty.Name))
                        {

                            if (ruleProperty.Value is JValue)
                            {
                                var dataValue = dataObjectProperty.Value as JValue;
                                var ruleValue = ruleProperty.Value as JValue;
                                var result = ValidateSingleData(op, dataValue, ruleValue);
                                if (result.IsValid == false)
                                {
                                    results.Add(result);
                                }
                            }
                            else if (ruleProperty.Value is JArray)
                            {
                                var dataArray = dataObjectProperty.Value as JArray;
                                var ruleArray = ruleProperty.Value as JArray;
                                var result = ValidateArrayData(op, new List<JToken> { dataArray }, ruleArray);
                                if (result.IsValid == false)
                                {
                                    results.Add(result);
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }

        private ValidationResult ValidateArrayData(string op, IEnumerable<JToken> dataTokens, JArray ruleValueArray)
        {
            string description = string.Empty;
            bool isValid = true;

            if (op.Equals("$all"))
            {
                bool isMissingAny = false;

                foreach (var ruleToken in ruleValueArray)
                {
                    if (ruleToken is JValue)
                    {
                        var ruleValue = ruleToken as JValue;
                        var ruleValueStr = ruleValue.ToString();

                        bool found = false;

                        foreach (var dataToken in dataTokens)
                        {
                            if (dataToken is JArray)
                            {
                                var dataArray = dataToken as JArray;

                                foreach (var dataArrayToken in dataArray)
                                {
                                    var dataValueStr = ((JValue)dataArrayToken).ToString();

                                    if (ruleValueStr == dataValueStr)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!found)
                        {
                            isMissingAny = true;
                            break;
                        }
                    }
                }

                if (isMissingAny)
                {
                    description = $"Not all values are present";
                    isValid = false;
                }
            }
            else if (op.Equals("$in") || op.Equals("$nin"))
            {
                bool dataValueIsInRuleArray = false;

                foreach (var ruleToken in ruleValueArray.OfType<JValue>())
                {
                    var ruleValueStr = ruleToken.ToString();
                    var values = dataTokens.OfType<JValue>();

                    foreach (var value in values)
                    {
                        var valueStr = value.Value<string>();
                        if (valueStr == ruleValueStr)
                        {
                            dataValueIsInRuleArray = true;
                            break;
                        }
                    }
                    if (dataValueIsInRuleArray)
                    {
                        break;
                    }
                }

                if (!dataValueIsInRuleArray && op.Equals("$in"))
                {
                    description = $"Value was not in the expected set of values";
                    isValid = false;
                }
                else if (dataValueIsInRuleArray && op.Equals("$nin"))
                {
                    description = $"Invalid value detected";
                    isValid = false;                    
                }
            }
            else if (op.Equals("$mod"))
            {
                var valuesArray = ruleValueArray.OfType<JToken>();

                if (valuesArray.Count() == 2)
                {
                    int modulo = valuesArray.ElementAt(0).Value<int>();
                    int expectedRemainder = valuesArray.ElementAt(1).Value<int>();

                    foreach (var dataItem in dataTokens.OfType<JValue>())
                    {
                        int? numerator = dataItem.Value<int>();

                        if (numerator.HasValue)
                        {
                            int actualRemainder = numerator.Value % modulo;
                            if (actualRemainder != expectedRemainder)
                            {
                                description = $"Expected remainder doesn't match the actual remainder";
                                isValid = false;
                                break;
                            }
                        }
                        else
                        {
                            description = $"Numerator could not be parsed";
                            isValid = false;
                            break;
                        }
                    }
                }
                else
                {
                    description = $"Modulo rule doesn't have two values as expected";
                    isValid = false;
                }
            }
            else if (op.Equals("$size"))
            {
                string expectedSizeStr = ruleValueArray.FirstOrDefault().Value<string>();

                bool success = int.TryParse(expectedSizeStr, out int expectedSize);

                if (success)
                {
                    foreach (var dataItem in dataTokens.OfType<JArray>())
                    {
                        int actualSize = dataItem.Count;
                        if (actualSize != expectedSize)
                        {
                            description = $"Expected size didn't match actual size";
                            isValid = false;
                            break;
                        }
                    }
                }
                else
                {
                    description = $"Expected size could not be parsed as an integer in the rules configuration";
                    isValid = false;
                }
            }

            var result = new ValidationResult(isValid);
            if (isValid == false)
            {
                result.Description = description;
            }
            return result;
        }

        private ValidationResult ValidateSingleData(string op, JValue dataValue, JValue ruleValue)
        {
            string description = string.Empty;
            string dataValueStr = dataValue != null ? dataValue.ToString() : null;
            string dataValuePath = dataValue != null ? dataValue.Path : "";
            string ruleValueStr = ruleValue.ToString();
            bool isValid = true;

            if (op.Equals("$eq"))
            {
                dataValueStr = dataValueStr ?? "null";

                if (ruleValueStr != dataValueStr)
                {
                    isValid = false;
                    description = $"Value '{dataValueStr}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                }
            }
            else if (op.Equals("$exists"))
            {
                if (ruleValueStr.Equals("true", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(dataValueStr))
                {
                    isValid = false;
                    description = $"Property '{ruleValueStr}' doesn't exist when it should";
                }
                else if (ruleValueStr.Equals("false", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataValueStr))
                {
                    isValid = false;
                    description = $"Property '{ruleValueStr}' exists when it shouldn't";
                }
            }
            else if (op.Equals("$ne"))
            {
                dataValueStr = dataValueStr ?? "null";

                if (ruleValueStr == dataValueStr)
                {
                    isValid = false;
                    description = $"Value '{dataValueStr}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                }
            }
            else if (op.Equals("$gt") || op.Equals("$gte") || op.Equals("$lt") || op.Equals("$lte"))
            {
                if (dataValue.Type == JTokenType.Integer)
                {
                    int dataValueInt = (int)dataValue;
                    int ruleValueInt = (int)ruleValue;

                    switch (op)
                    {
                        case "$gt":
                            isValid = dataValueInt > ruleValueInt;
                            break;
                        case "$gte":
                            isValid = dataValueInt >= ruleValueInt;
                            break;
                        case "$lt":
                            isValid = dataValueInt < ruleValueInt;
                            break;
                        case "$lte":
                            isValid = dataValueInt <= ruleValueInt;
                            break;
                    }

                    if (!isValid)
                    {
                        description = $"Value '{dataValue}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                    }
                }
                else if (dataValue.Type == JTokenType.Float)
                {
                    double dataValueDouble = (double)dataValue;
                    double ruleValueDouble = (double)ruleValue;

                    switch (op)
                    {
                        case "$gt":
                            isValid = dataValueDouble > ruleValueDouble;
                            break;
                        case "$gte":
                            isValid = dataValueDouble >= ruleValueDouble;
                            break;
                        case "$lt":
                            isValid = dataValueDouble < ruleValueDouble;
                            break;
                        case "$lte":
                            isValid = dataValueDouble <= ruleValueDouble;
                            break;
                    }

                    if (!isValid)
                    {
                        description = $"Value '{dataValue}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                    }
                }
                else if (dataValue.Type == JTokenType.Boolean)
                {
                    bool dataValueBool = (bool)dataValue;
                    bool ruleValueBool = (bool)ruleValue;

                    switch (op)
                    {
                        case "$gt":
                            isValid = dataValueBool == true && ruleValueBool == false;
                            break;
                        case "$gte":
                            isValid = !(dataValueBool == false && ruleValueBool == true);
                            break;
                        case "$lt":
                            isValid = dataValueBool == false && ruleValueBool == true;
                            break;
                        case "$lte":
                            isValid = !(dataValueBool == true && ruleValueBool == false);
                            break;
                    }

                    if (!isValid)
                    {
                        description = $"Value '{dataValue}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                    }
                }
                else if (dataValue.Type == JTokenType.String)
                {
                    var stringComparisonResult = dataValue.CompareTo(ruleValue);

                    switch (op)
                    {
                        case "$gt":
                            isValid = stringComparisonResult > 0;
                            break;
                        case "$gte":
                            isValid = stringComparisonResult >= 0;
                            break;
                        case "$lt":
                            isValid = stringComparisonResult < 0;
                            break;
                        case "$lte":
                            isValid = stringComparisonResult <= 0;
                            break;
                    }

                    if (!isValid)
                    {
                        description = $"Value '{dataValue}' for field at path '{dataValuePath}' must be {GetOperatorDescription(op)} '{ruleValue}'";
                    }
                }
            }
            else if (op.Equals("$regex"))
            {
                dataValueStr = dataValueStr ?? "";

                Regex _regex = new Regex(ruleValueStr);

                if (!_regex.IsMatch(dataValueStr))
                {
                    isValid = false;
                    description = $"Value '{dataValueStr}' for field at path '{dataValuePath}' did not match regular expression {ruleValueStr}";
                }
            }
            else if (op.Equals("$type"))
            {
                dataValueStr = dataValueStr ?? "";

                switch (ruleValueStr.ToUpper())
                {
                    case "STRING":
                        isValid = dataValue.Type == JTokenType.String;
                        break;
                    case "NUMBER":
                        isValid = (dataValue.Type == JTokenType.Integer || dataValue.Type == JTokenType.Float);
                        break;
                    case "BOOLEAN":
                        isValid = dataValue.Type == JTokenType.Boolean;
                        break;
                    case "ARRAY":
                        isValid = dataValue.Type == JTokenType.Array;
                        break;
                    case "OBJECT":
                        isValid = dataValue.Type == JTokenType.Object;
                        break;
                    case "NULL":
                        isValid = dataValue.Type == JTokenType.Null;
                        break;
                    case "BYTES":
                        isValid = dataValue.Type == JTokenType.Bytes;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (!isValid)
                {
                    description = $"Value '{dataValueStr}' did not match expected type {dataValue.Type}";
                }
            }

            var result = new ValidationResult(isValid);
            if (isValid == false)
            {
                result.Description = description;
            }
            return result;
        }

        private string GetOperatorDescription(string op)
        {
            switch (op)
            {
                case "$eq":
                    return "equal to";
                case "$ne":
                    return "not equal to";
                case "$gt":
                    return "greater than";
                case "$gte":
                    return "greater than or equal to";
                case "$lt":
                    return "less than";
                case "$lte":
                    return "less than or equal to";
                default:
                    return "unknown operator";
            }
        }
    }
}
