using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Foundation.RulesEngine.Models
{
    public sealed class ValidationResults
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();
        public bool IsValid => !(Results.Any(r => r.IsValid == false));

        public IEnumerable<ValidationResult> Results => _results.AsEnumerable();

        public ValidationResults(IEnumerable<ValidationResult> results)
        {
            foreach (var result in results)
            {
                _results.Add(result);
            }
        }
    }

    [DebuggerDisplay("{IsValid} : {Description}")]
    public sealed class ValidationResult
    {
        public bool IsValid { get; private set; } = false;
        public string Description { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Expected { get; set; } = string.Empty;
        public string Actual { get; set; } = string.Empty;

        public ValidationResult(bool isValid)
        {
            IsValid = isValid;
        }
    }
}