using System.Runtime.Serialization;

namespace Contract.Models;

public enum OrderTypeEnum
{
    [EnumMember(Value = "Ascending")]
    Ascending = 1,
    [EnumMember(Value = "Descending")]
    Descending = 2
}