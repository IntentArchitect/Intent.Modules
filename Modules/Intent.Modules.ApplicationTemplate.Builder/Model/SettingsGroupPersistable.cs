#nullable disable
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Intent.IArchitect.Agent.Persistence.Model.Module;

namespace Intent.Modules.ApplicationTemplate.Builder.Model
{
    public class SettingsGroupPersistable : IEquatable<SettingsGroupPersistable>
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlArray("settings")]
        [XmlArrayItem("setting")]
        public List<SettingFieldPersistable> Settings { get; set; }

        [XmlAttribute("type")]
        public SettingGroupPersistableType Type { get; set; } = SettingGroupPersistableType.ApplicationSettings;
        public bool ShouldSerializeType() => Type != default;

        public bool Equals(SettingsGroupPersistable other)
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
            return Equals((SettingsGroupPersistable) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}