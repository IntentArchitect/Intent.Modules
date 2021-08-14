using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common
{
    public enum SortChildrenOptions
    {
        [XmlEnum("manually")]
        Manually = 0,
        [XmlEnum("by-type-then-manually")]
        SortByTypeThenManually = 1,
        [XmlEnum("by-type-then-by-name")]
        SortByTypeAndName = 2,
        [XmlEnum("by-name")]
        SortByName = 3
    }

    public class PackageSettingsPersistable
    {
        private List<TypeOrderPersistable> _typeOrder;

        [XmlAttribute("type")]
        public string SpecializationType { get; set; }

        [XmlAttribute("typeId")]
        public string SpecializationTypeId { get; set; }

        [XmlElement("sortChildren")]
        public SortChildrenOptions SortChildren { get; set; }

        [XmlElement("defaultName")]
        public string DefaultName { get; set; }

        [XmlElement("icon")]
        public IconModelPersistable Icon { get; set; }

        [XmlArray("requiredPackages")]
        [XmlArrayItem("package")]
        public string[] RequiredPackages { get; set; } = new string[0];

        [XmlArray("creationOptions")]
        [XmlArrayItem("option")]
        public List<ElementCreationOption> CreationOptions { get; set; }

        [XmlArray("scriptOptions")]
        [XmlArrayItem("option")]
        public List<RunScriptOption> ScriptOptions { get; set; }

        [XmlArray("typeOrder")]
        [XmlArrayItem("type")]
        public List<TypeOrderPersistable> TypeOrder
        {
            get => _typeOrder;
            set
            {
                if (value == null)
                {
                    _typeOrder = null;
                    return;
                }

                _typeOrder = value;
                foreach (var typeOrder in value.ToList().Where(x => !string.IsNullOrWhiteSpace(x.Order)))
                {
                    _typeOrder.Remove(typeOrder);
                    _typeOrder.Insert(Math.Max(Math.Min(int.Parse(typeOrder.Order), _typeOrder.Count), 0), typeOrder);
                }
                UpdateTypesOrdering();
            }
        }

        [XmlArray("macros")]
        [XmlArrayItem("macro")]
        public List<ElementMacroPersistable> Macros { get; set; }

        public void AddType(TypeOrderPersistable typeOrder)
        {
            TypeOrder.Add(typeOrder);
            UpdateTypesOrdering();
        }

        private void UpdateTypesOrdering()
        {
            _typeOrder = _typeOrder.Select((x, index) => new TypeOrderPersistable()
            {
                Type = x.Type,
                Order = !string.IsNullOrWhiteSpace(x.Order) ? x.Order : index.ToString()
            }).OrderBy(x => int.Parse(x.Order)).ThenBy(x => x.Type).ToList();
        }
    }
}