using System;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class TypeOrderPersistable : IEquatable<TypeOrderPersistable>
{
    [XmlAttribute("order")]
    public string OrderSerialized
    {
        get => Order?.ToString();
        set => Order = int.TryParse(value, out var order) ? order : null;
    }
    public bool ShouldSerializeOrderSerialized() => Order.HasValue;

    [XmlIgnore]
    public int? Order { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; }

    [XmlAttribute("typeId")]
    public string TypeId { get; set; }


    public override string ToString()
    {
        return $"{nameof(Type)} = '{Type}', " +
               $"{nameof(TypeId)} = '{TypeId}', " +
               $"{nameof(Order)} = '{Order}'";
    }

    public bool Equals(TypeOrderPersistable other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return TypeId == other.TypeId;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TypeOrderPersistable)obj);
    }

    public override int GetHashCode()
    {
        return (TypeId != null ? TypeId.GetHashCode() : 0);
    }
}