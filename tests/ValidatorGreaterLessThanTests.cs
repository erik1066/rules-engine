using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Foundation.RulesEngine.Validators;

namespace Foundation.RulesEngine.Tests
{
    public partial class ValidatorEqualityTests
    {
        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$gt': { '$.contact.age': 18 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$gt': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$gt': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'name': 'B' } }", "{ '$gt': { '$.contact.name': 'A' } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'Ben', 'age': 33, 'score': 0.75, 'enabled': true }, { 'name': 'John', 'age': 28, 'score': 0.6, 'enabled': true } ] }", "{ '$gt': { '$.contacts[*]': { 'age': 18, 'score': 0.5, 'enabled': false } } }")]
        public void GreaterThan_IsValid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
            Assert.True(result.Results.Count() == 0);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$gt': { '$.contact.age': 35 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$gt': { '$.contact.score': 0.7 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$gt': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$gt': { '$.contact.enabled': true } }")]
        public void GreaterThan_Simple_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 1);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$gte': { '$.contact.age': 18 } }")]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$gte': { '$.contact.age': 33 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$gte': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$gte': { '$.contact.score': 0.6 } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$gte': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$gte': { '$.contact.enabled': true } }")]
        [InlineData(@"{ 'contact': { 'name': 'B' } }", "{ '$gte': { '$.contact.name': 'A' } }")]
        [InlineData(@"{ 'contact': { 'name': 'B' } }", "{ '$gte': { '$.contact.name': 'B' } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'Ben', 'age': 33, 'score': 0.75, 'enabled': true }, { 'name': 'John', 'age': 28, 'score': 0.6, 'enabled': true } ] }", "{ '$gte': { '$.contacts[*]': { 'age': 18, 'score': 0.5, 'enabled': false } } }")]
        public void GreaterThanOrEqualTo_IsValid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
            Assert.True(result.Results.Count() == 0);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$gte': { '$.contact.age': 35 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$gte': { '$.contact.score': 0.7 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$gte': { '$.contact.enabled': true } }")]
        public void GreaterThanOrEqualTo_Simple_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 1);
        }




        [Theory]
        [InlineData(@"{ 'contact': { 'age': 17 } }", "{ '$lt': { '$.contact.age': 18 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.1 } }", "{ '$lt': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$lt': { '$.contact.enabled': true } }")]
        [InlineData(@"{ 'contact': { 'name': 'ABCD' } }", "{ '$lt': { '$.contact.name': 'D' } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'B', 'age': 14, 'score': 0.2, 'enabled': false }, { 'name': 'D', 'age': 1, 'score': 0.4, 'enabled': false } ] }", "{ '$lt': { '$.contacts[*]': { 'age': 18, 'score': 0.5, 'enabled': true } } }")]
        public void LessThan_IsValid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
            Assert.True(result.Results.Count() == 0);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$lt': { '$.contact.age': 30 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$lt': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$lt': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$lt': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$lt': { '$.contact.enabled': true } }")]
        public void LessThan_Simple_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 1);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$lte': { '$.contact.age': 33 } }")]
        [InlineData(@"{ 'contact': { 'age': 18 } }", "{ '$lte': { '$.contact.age': 33 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$lte': { '$.contact.score': 0.6 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.5 } }", "{ '$lte': { '$.contact.score': 0.6 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$lte': { '$.contact.enabled': true } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$lte': { '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contact': { 'name': 'B' } }", "{ '$lte': { '$.contact.name': 'B' } }")]
        [InlineData(@"{ 'contact': { 'name': 'A' } }", "{ '$lte': { '$.contact.name': 'B' } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'B', 'age': 15, 'score': 0.4, 'enabled': false }, { 'name': 'C', 'age': 18, 'score': 0.5, 'enabled': false } ] }", "{ '$lte': { '$.contacts[*]': { 'age': 18, 'score': 0.5, 'enabled': false } } }")]
        public void LessThanOrEqualTo_IsValid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
            Assert.True(result.Results.Count() == 0);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'age': 33 } }", "{ '$lte': { '$.contact.age': 31 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$lte': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'enabled': true } }", "{ '$lte': { '$.contact.enabled': false } }")]
        public void LessThanOrEqualTo_Simple_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 1);
        }
    }
}