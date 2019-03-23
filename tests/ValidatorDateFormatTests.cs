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
    public sealed class ValidatorDateFormatTests
    {
        private const string _dateformat_rules_01 = 
        @"{
            '$dateFormat': {
                '$.contacts[*]': {
                    'dob': 'yyyyMMdd'
                }
            }
        }";

        [Theory]
        [InlineData("{ 'contacts': [ { 'dob': '20190101' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20191231' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20550505' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20160229' } ] }", _dateformat_rules_01)]
        public void DateFormat_IsValid_Tests(string data, string rules)
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
        [InlineData("{ 'contacts': [ { 'dob': '20190001' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20190100' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20191232' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20170229' } ] }", _dateformat_rules_01)]
        public void DateFormat_IsInvalid_DateDoesntExist_Tests(string data, string rules)
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
        [InlineData("{ 'contacts': [ { 'dob': '201901011' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '2019000' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20190' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '201' } ] }", _dateformat_rules_01)]
        [InlineData("{ 'contacts': [ { 'dob': '20' } ] }", _dateformat_rules_01)]
        public void DateFormat_IsInvalid_NotADate_Tests(string data, string rules)
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