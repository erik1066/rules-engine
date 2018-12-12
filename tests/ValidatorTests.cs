using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Foundation.RulesEngine;

namespace Foundation.RulesEngine.Tests
{
    public partial class ValidatorTests
    {
        [Theory]
        [InlineData(@"{ 'contact': { 'name': 'AAA' } }", "{ '$eq': { '$.contact.name': 'AAA' } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33 } }", "{ '$eq': { '$.contact.name': 'AAA', '$.contact.age': 33 } }")]
        [InlineData(@"{ 'contact': { 'name': 'AAA', 'age': 33, 'score': 0.6, 'enabled': true } }", "{ '$eq': { '$.contact.name': 'AAA', '$.contact.age': 33, '$.contact.score': 0.6, '$.contact.enabled': true } }")]
        public void Equals_IsValid_Tests(string obj, string rules)
        {
            var jsonObject = JObject.Parse(obj);
            var jsonRules = JObject.Parse(rules);

            var v = new SimpleValidator(jsonRules);

            var result = v.Validate(jsonObject);
            Console.WriteLine(result);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(@"{ 'contact': { 'name': 'BBB' } }", "{ '$eq': { '$.contact.name': 'CCC' } }")]
        public void Equals_IsInvalid_Tests(string obj, string rules)
        {
            var jsonObject = JObject.Parse(obj);
            var jsonRules = JObject.Parse(rules);

            var v = new SimpleValidator(jsonRules);

            var result = v.Validate(jsonObject);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
        }
    }
}