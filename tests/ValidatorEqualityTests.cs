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
        [InlineData(@"{ 'contact': { 'name': 'AAA' } }", "{ '$eq': { '$.contact.name': 'AAA' } }")]
        [InlineData(@"{ 'contact': { 'name': null } }", "{ '$eq': { '$.contact.name': null } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33 } }", "{ '$eq': { '$.contact.name': 'AAA', '$.contact.age': 33 } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33, 'score': 0.6, 'enabled': true } }", "{ '$eq': { '$.contact.name': 'AAA', '$.contact.age': 33, '$.contact.score': 0.6, '$.contact.enabled': true } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'CCC', 'age': 33, 'score': 0.6, 'enabled': true } ] }", @"{ '$eq': { '$.contacts[*]': { 'age': 33, 'score': 0.6, 'enabled': true } } }")]
        public void Equals_IsValid_Tests(string data, string rules)
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
        [InlineData(@"{ 'contact': { 'age': 35 } }", "{ '$eq': { '$.contact.name': 'CCC' } }")]
        [InlineData(@"{ 'contact': { 'name': 'BBB' } }", "{ '$eq': { '$.contact.name': 'CCC' } }")]
        [InlineData(@"{ 'contact': { 'name': 'BBB' } }", "{ '$eq': { '$.contact.name': null } }")]
        [InlineData(@"{ 'contact': { 'name': null } }", "{ '$eq': { '$.contact.name': 'CCC' } }")]
        [InlineData(@"{ 'contact': { 'age': 25 } }", "{ '$eq': { '$.contact.age': 35 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$eq': { '$.contact.score': 0.5 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$eq': { '$.contact.enabled': true } }")]
        public void Equals_Simple_IsInvalid_Tests(string data, string rules)
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
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$eq': { '$.contacts[*]': { 'name': 'CCC', 'age': 33, 'score': 0.6, 'enabled': true } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$eq': { '$.contacts[*]': { 'name': 'CCC', 'age': 35, 'score': 0.6, 'enabled': false } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$eq': { '$.contacts[*]': { 'name': 'CCC', 'age': 35, 'score': 0.61, 'enabled': true } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': null, 'age': 35 } ] }", @"{ '$eq': { '$.contacts[*]': { 'name': 'CCC', 'age': 36 } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35 } ] }", @"{ '$eq': { '$.contacts[*]': { 'name': null, 'age': 36 } } }")]
        public void Equals_Complex_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 2);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'name': 'AAA' } }", "{ '$ne': { '$.contact.name': 'BBB' } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA' } }", "{ '$ne': { '$.contact.name': null } }")]
        [InlineData(@"{ 'contact': { 'name': null } }", "{ '$ne': { '$.contact.name': 'BBB' } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33 } }", "{ '$ne': { '$.contact.name': 'BBB', '$.contact.age': 35 } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33, 'score': 0.6, 'enabled': true } }", "{ '$ne': { '$.contact.name': 'BBB', '$.contact.age': 35, '$.contact.score': 0.5, '$.contact.enabled': false } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'CCC', 'age': 33, 'score': 0.6, 'enabled': true } ] }", @"{ '$ne': { '$.contacts[*]': { 'age': 31, 'score': 0.5, 'enabled': false } } }")]
        public void NotEquals_IsValid_Tests(string data, string rules)
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
        [InlineData(@"{ 'contact': { 'name': 'BBB' } }", "{ '$ne': { '$.contact.name': 'BBB' } }")]
        [InlineData(@"{ 'contact': { 'name': null } }", "{ '$ne': { '$.contact.name': null } }")]
        [InlineData(@"{ 'contact': { 'age': 25 } }", "{ '$ne': { '$.contact.age': 25 } }")]
        [InlineData(@"{ 'contact': { 'score': 0.6 } }", "{ '$ne': { '$.contact.score': 0.6 } }")]
        [InlineData(@"{ 'contact': { 'enabled': false } }", "{ '$ne': { '$.contact.enabled': false } }")]
        public void NotEquals_Simple_IsInvalid_Tests(string data, string rules)
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
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$ne': { '$.contacts[*]': { 'name': 'AAA', 'age': 35, 'score': 0.5, 'enabled': false } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$ne': { '$.contacts[*]': { 'name': 'CCC', 'age': 35, 'score': 0.6, 'enabled': false } } }")]
        [InlineData(@"{ 'contacts': [ { 'name': 'AAA', 'age': 35, 'score': 0.6, 'enabled': true } ] }", @"{ '$ne': { '$.contacts[*]': { 'name': 'CCC', 'age': 36, 'score': 0.6, 'enabled': true } } }")]
        public void NotEquals_Complex_IsInvalid_Tests(string data, string rules)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == 2);
        }
    }
}