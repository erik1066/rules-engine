// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text;

// using Newtonsoft.Json.Linq;

// using Foundation.RulesEngine.Commands;

// namespace Foundation.RulesEngine
// {
//     public class CompoundValidationResult : ValidationResult
//     {
//         private List<ValidationResult> _results = new List<ValidationResult>();

//         public CompoundValidationResult() 
//         {
// 		    IsValid = true;
// 	    }

//         public CompoundValidationResult(string rule, string obj, ICommand command) 
//         {
//             IsValid = true;

//             Rule = rule;
//             Object = obj;
//             Command = command.GetKeyword();

//             if (command is SingleCommand)
//             {
//                 var sc = (SingleCommand) command;
//                 Description = sc.Description;
//                 Comment = sc.Comment;
//             }
//         }

//         public void Add(ValidationResult result)
//         {
//             if (result.IsValid == false)
//             {
//                 IsValid = false;
//             }
//             _results.Add(result);
//         }

//         public void AddMany(IEnumerable<ValidationResult> results)
//         {
//             foreach (var result in results)
//             {
//                 Add(result);
//             }
//         }

//         public IEnumerable<ValidationResult> GetValidationResults() => _results.AsReadOnly();

//         public IEnumerable<ValidationResult> Flatten() 
//         {
//             List<ValidationResult> singleResults = new List<ValidationResult>();
//             foreach (var result in _results) 
//             {
//                 if (result is CompoundValidationResult) 
//                 {
//                     if ("$or".Equals(result.Command, StringComparison.OrdinalIgnoreCase) ||
//                         "$and".Equals(result.Command, StringComparison.OrdinalIgnoreCase) ||
//                         ((CompoundValidationResult) result).GetValidationResults().Count() > 1) 
//                     {                        
//                         singleResults.AddRange(((CompoundValidationResult) result).Flatten());
//                     }
//                     else 
//                     {
//                         singleResults.Add(result);
//                     }
//                 }
//                 else 
//                 {
//                     singleResults.Add((ValidationResult) result);
//                 }
//             }
//             return singleResults;
//         }

//         public override string ToString() 
//         {
//             StringBuilder sb = new StringBuilder();
//             sb.Append("Rule: ");
//             sb.Append(Rule);
//             sb.Append("\nObject: ");
//             sb.Append(Object);
//             sb.Append("\nCommand: ");
//             sb.Append(Command);
//             sb.Append("\nComment: ");
//             sb.Append(Comment);
//             sb.Append("\nDescription: ");
//             sb.Append(Description);
//             sb.Append("\nIs Valid? ");
//             sb.Append(IsValid);
//             sb.Append("\n");

//             foreach (var result in _results) 
//             {
//                 sb.Append(result.ToString(1));
//             }

//             return sb.ToString();
//         }

//         public override string ToString(int index) 
//         {
//             StringBuilder sb = new StringBuilder();
//             sb.Append("Rule: ".PadLeft(index * 2, ' '));
//             sb.Append(Rule);
//             sb.Append("\n");
//             sb.Append("Object: ".PadLeft(index * 2, ' '));
//             sb.Append(Object);
//             sb.Append("\n");
//             sb.Append("Command: ".PadLeft(index * 2, ' '));
//             sb.Append(Command);
//             sb.Append("\n");
//             sb.Append("Comment: ".PadLeft(index * 2, ' '));
//             sb.Append(Comment);
//             sb.Append("\n");
//             sb.Append("Description: ".PadLeft(index * 2, ' '));
//             sb.Append(Description);
//             sb.Append("\n");
//             sb.Append("Is Valid? ".PadLeft(index * 2, ' '));
//             sb.Append(IsValid);
//             sb.Append("\n");

//             foreach (var result in _results) 
//             {
//                 sb.Append(result.ToString(index + 1));
//                 sb.Append("\n");
//             }

//             return sb.ToString();
//         }
//     }
// }