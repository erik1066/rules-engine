using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Foundation.RulesEngine.Validators;

namespace Foundation.RulesEngine.Tests
{
    public class ValidatorComplexTests
    {
        private readonly string _object1;
        private readonly string _rules1;

        public ValidatorComplexTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            var path = Path.Combine(basePath, "resources", "objects", "hl7-002.json");
            _object1 = File.ReadAllText(path);

            path = Path.Combine(basePath, "resources", "rules", "003.json");
            _rules1 = File.ReadAllText(path);
        }

        [Fact]
        public void Validate()
        {
            var jsonData = JObject.Parse(_object1);
            var jsonRules = JObject.Parse(_rules1);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
            Assert.True(result.Results.Count() == 0);
        }
    }
}