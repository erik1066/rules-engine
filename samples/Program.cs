using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Foundation.RulesEngine.Validators;
using System.Diagnostics;
using System.Linq;

namespace samples
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidateSimple();
            ValidateComplex();
        }

        static void ValidateSimple()
        {
            string rules = "{ '$eq': { '$.contact.name': 'AAA' } }";
            JObject jsonRules = JObject.Parse(rules);

            var v = new Validator();

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 1; i++)
            {
                string data = "{ 'contact': { 'name': 'AAA" + i.ToString() + "' } }";
                JObject jsonData = JObject.Parse(data);
                var result = v.Validate(jsonData, jsonRules);
            }

            sw.Stop();
            Console.WriteLine($"EQUALITY:\t {sw.Elapsed.TotalMilliseconds.ToString("N0")} ms");
        }

        static void ValidateComplex()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string path = Path.Combine(basePath, "resources", "objects", "hl7-002.json");
            string object1 = File.ReadAllText(path);

            path = Path.Combine(basePath, "resources", "rules", "003.json");
            string rules1 = File.ReadAllText(path);

            var jsonData = JObject.Parse(object1);
            var jsonRules = JObject.Parse(rules1);

            var v = new Validator();

            var sw = new Stopwatch();
            sw.Start();
            
            for(int i = 0; i < 1; i++)
            {
                var result = v.Validate(jsonData, jsonRules);

                Console.WriteLine($"Errors: {result.Results.Count()}");
                foreach (var problem in result.Results)
                {
                    Console.WriteLine("Error: " + problem.Description);
                }
            }

            sw.Stop();
            Console.WriteLine($"COMPLEX:\t {sw.Elapsed.TotalMilliseconds.ToString("N0")} ms");
        }
    }
}
