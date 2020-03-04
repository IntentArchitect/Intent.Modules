using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class ElementSetting
    {
        public const string RequiredSpecializationType = "Element Settings";

        public ElementSetting(IElement element) 
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            SpecializationType = element.Name;
            Icon = new IconModel(element.GetStereotype("Icon (Full)"));
            ExpandedIcon = new IconModel(element.GetStereotype("Icon (Full, Expanded)"));
            AllowRename = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Rename");
            AllowAbstract = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Abstract");
            AllowGenericTypes = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Generic Types");
            AllowMapping = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Mapping");
            AllowSorting = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Sorting");
            AllowFindInView = element.GetStereotypeProperty<bool?>("Addition Properties", "Allow Find in View");
            AttributeSettings = element.ChildElements
                .Where(x => x.SpecializationType == AttributeSetting.RequiredSpecializationType)
                .Select(x => new AttributeSetting(x)).ToList();
            CreationOptions = element.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new CreationOption(x)).ToList();
        }

        public string SpecializationType { get; set; }

        public IconModel Icon { get; set; }

        public IconModel ExpandedIcon { get; set; }

        public bool? AllowRename { get; set; }

        public bool? AllowAbstract { get; set; }

        public bool? AllowGenericTypes { get; set; }

        public bool? AllowMapping { get; set; }

        public bool? AllowSorting { get; set; }

        public bool? AllowFindInView { get; set; }

        public List<TypeOrder> TypeOrder { get; set; }

        public IList<AttributeSetting> AttributeSettings { get; set; }
        public IList<OperationSetting> OperationSettings { get; set; }
        public IList<CreationOption> CreationOptions { get; set; }
    }
}