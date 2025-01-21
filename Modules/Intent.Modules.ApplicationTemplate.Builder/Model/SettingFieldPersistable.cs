#nullable disable
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Intent.Modules.ApplicationTemplate.Builder.Model;

public class SettingFieldPersistable : IEquatable<SettingFieldPersistable>
{
    [XmlAttribute("id")]
    public string Id { get; set; }

    [XmlAttribute("title")]
    public string Title { get; set; }

    [XmlElement("hint")]
    public string Hint { get; set; }

    [XmlAttribute("type")]
    public ModuleSettingControlType ControlType { get; set; }

    [XmlElement("defaultValue")]
    public string DefaultValue { get; set; }

    [XmlElement("isRequiredFunction")]
    public string IsRequiredFunction { get; set; }

    [XmlElement("isActiveFunction")]
    public string IsActiveFunction { get; set; }

    [XmlElement("isRequired")]
    public bool IsRequired { get; set; }

    [XmlArray("options")]
    [XmlArrayItem("option")]
    public List<SettingFieldOptionPersistable> Options { get; set; }

    public bool Equals(SettingFieldPersistable other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SettingFieldPersistable)obj);
    }

    public override int GetHashCode()
    {
        return (Id != null ? Id.GetHashCode() : 0);
    }
}