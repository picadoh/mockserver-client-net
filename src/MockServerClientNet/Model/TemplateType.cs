namespace MockServerClientNet.Model;

using System.Runtime.Serialization;

public enum TemplateType
{
    [EnumMember(Value = "VELOCITY")]
    Velocity,
    [EnumMember(Value = "MUSTACHE")]
    Mustache,
}
