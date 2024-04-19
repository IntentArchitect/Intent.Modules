using System;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class TargetTypePersistable : IEquatable<TargetTypePersistable>
{
    [XmlText]
    public string TypeDeprecated
    {
        get => Type;
        set => Type = value;
    }

    public bool ShouldSerializeTypeDeprecated() => false;

    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("typeId")]
    public string TypeId { get; set; }

    public override string ToString()
    {
        return $"{nameof(Type)} = '{Type}', " +
               $"{nameof(TypeId)} = '{TypeId}'";
    }

    public bool Equals(TargetTypePersistable other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && TypeId == other.TypeId;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TargetTypePersistable)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, TypeId);
    }
}