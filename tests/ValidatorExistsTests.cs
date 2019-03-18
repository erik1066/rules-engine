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
    public partial class ValidatorExistsTests
    {
        private const string _all_rules_01 = 
        @"{
            '$exists': {
                '$.contact.name': true,
                '$.contact': true,
                '$.mycontact': false,
                '$.contact.age': false
            }
        }";

        private const string _all_rules_02 =
        @"{
            '$exists': {
                '$.contact.age': true,
                '$.contact': false
            }
        }";

        private const string _all_rules_03 =
        @"{{
            '$exists': {
                '$.contacts': true,
                '$.contacts[*]': {
                    'name': true,
                    'age': false
                }
            }
        }";

        private const string _all_rules_04 =
        @"{
            '$exists': {
                '$.contacts': true,
                '$.contacts[*]': {
                    'name': true
                }
            }
        }";

        private const string _all_rules_05 =
        @"{
            '$exists': {
                '$.contacts': true,
                '$.contacts[*]': {
                    'sex': false
                }
            }
        }";

        [Theory]
        [InlineData("{ 'contact': { 'name': 'AAA' } }", _all_rules_01)]
        [InlineData("{ 'contacts': [ { 'name': 'AAA', 'age': 33 }, { 'name': 'BBB' } ] }", _all_rules_04)]
        [InlineData("{ 'contacts': [ { 'name': 'AAA', 'age': 33 }, { 'name': 'BBB' } ] }", _all_rules_05)]
        public void Exists_IsValid_Tests(string data, string rules)
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
        [InlineData("{ 'contact': { 'sex': 'M' } }", _all_rules_01, 1)]
        [InlineData("{ 'contact': { 'age': 23 } }", _all_rules_01, 2)]
        [InlineData("{ 'contact': { 'sex': 'M' } }", _all_rules_02, 2)]        
        // [InlineData("{ 'contacts': [ { 'name': 'AAA' }, { 'name': 'BBB' } ] }", _all_rules_02)]
        // [InlineData("{ 'contacts': [ { 'name': 'AAA', 'age': 33 }, { 'name': 'BBB' } ] }", _all_rules_04)]
        // [InlineData("{ 'contacts': [ { 'name': 'AAA', 'age': 33 }, { 'name': 'BBB' } ] }", _all_rules_05)]
        public void Exists_IsInvalid_Tests(string data, string rules, int expectedErrors)
        {
            var jsonData = JObject.Parse(data);
            var jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var result = v.Validate(jsonData, jsonRules);
            Console.WriteLine(result);

            Assert.False(result.IsValid);
            Assert.True(result.Results.Count() == expectedErrors);
        }
    }
}