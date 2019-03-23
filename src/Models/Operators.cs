using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Foundation.RulesEngine.Models
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Operator
    {
        Undefined = 0,
        [EnumMember(Value = "$all")]
        All,
        [EnumMember(Value = "$exists")]
        Exists,
        [EnumMember(Value = "$eq")]
        Equal,
        [EnumMember(Value = "$ne")]
        NotEqual,
        [EnumMember(Value = "$gt")]
        GreaterThan,
        [EnumMember(Value = "$gte")]
        GreaterThanOrEqualTo,
        [EnumMember(Value = "$lt")]
        LessThan,
        [EnumMember(Value = "$lte")]
        LessThanOrEqualTo,
        [EnumMember(Value = "$in")]
        In,
        [EnumMember(Value = "$nin")]
        NotIn,
        [EnumMember(Value = "$regex")]
        RegularExpression,
        [EnumMember(Value = "$mod")]
        Modulus,
        [EnumMember(Value = "$size")]
        Size,
        [EnumMember(Value = "$type")]
        Type
    }
}