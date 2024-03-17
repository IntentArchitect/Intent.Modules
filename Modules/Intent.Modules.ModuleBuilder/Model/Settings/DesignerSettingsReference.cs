using System;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public class DesignerSettingsReference : IEquatable<DesignerSettingsReference>
    {
        public static DesignerSettingsReference Create(string id, string name, string module)
        {
            return new DesignerSettingsReference
            {
                Id = id,
                Name = name,
                Module = module,
            };
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("module")]
        public string Module { get; set; }

        public bool Equals(DesignerSettingsReference other)
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
            return Equals((DesignerSettingsReference)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}