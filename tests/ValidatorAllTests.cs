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
    public partial class ValidatorAllTests
    {
        private const string _all_rules_01 = 
        @"{
            '$all': {
                '$.contact.hobbies': [
                    'Travel',
                    'Ski'
                ]
            }
        }";

        private const string _all_rules_02 =
        @"{
            '$all': {
                '$.contacts[*]': {
                    'hobbies': [
                        'Cinema',
                        'Ski'
                    ]
                }
            }
        }";

        [Theory]
        [InlineData(
            @"{
                'contact': {
                    'name': 'Ben',
                    'age': 33,
                    'score': 0.6,
                    'enabled': true,
                    'hobbies': [
                        'Cinema',
                        'Travel',
                        'Ski'
                    ]
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contacts': [
                    {
                        'name': 'Ben',
                        'age': 33,
                        'score': 0.5,
                        'enabled': true,
                        'ip': null,
                        'hobbies': [
                            'Cinema',
                            'Travel',
                            'Ski'
                        ]
                    },
                    {
                        'name': 'John',
                        'age': 30,
                        'score': 1,
                        'enabled': true,
                        'hobbies': [
                            'Cinema',
                            'Travel',
                            'Ski'
                        ]
                    }
                ]
            }", _all_rules_02
            )]
        public void All_IsValid_Tests(string data, string rules)
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
        [InlineData(
            @"{
                'contact': {
                    'hobbies': [
                        'Cinema',
                        'Ski'
                    ]
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contact': {
                    'hobbies': [
                        'Travel',
                        'Cinema'
                    ]
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contact': {
                    'hobbies': [
                        'Ski'
                    ]
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contact': {
                    'hobbies': []
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contact': {
                    'name': 'Ben',
                    'age': 33,
                    'score': 0.6,
                    'enabled': true,
                    'hobbies': [
                        'Cinema',
                        'Ski'
                    ]
                }
            }", _all_rules_01
            )]
        [InlineData(
            @"{
                'contacts': [
                    {
                        'name': 'Ben',
                        'age': 33,
                        'score': 0.5,
                        'enabled': true,
                        'ip': null,
                        'hobbies': [
                            'Travel',
                            'Ski'
                        ]
                    },
                    {
                        'name': 'John',
                        'age': 30,
                        'score': 1,
                        'enabled': true,
                        'hobbies': [
                            'Cinema',
                            'Travel',
                            'Ski'
                        ]
                    }
                ]
            }", _all_rules_02
            )]
        public void All_IsInvalid_Tests(string data, string rules)
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