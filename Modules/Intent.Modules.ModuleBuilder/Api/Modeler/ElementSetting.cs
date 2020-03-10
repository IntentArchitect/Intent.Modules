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
            Icon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full)"));
            ExpandedIcon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full, Expanded)"));
            AllowRename = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
            AllowAbstract = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Abstract");
            AllowGenericTypes = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Generic Types");
            AllowMapping = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Mapping");
            AllowSorting = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Sorting");
            AllowFindInView = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
            LiteralSettings = element.ChildElements
                .Where(x => x.SpecializationType == LiteralSetting.RequiredSpecializationType)
                .Select(x => new LiteralSetting(x)).ToList();
            AttributeSettings = element.ChildElements
                .Where(x => x.SpecializationType == AttributeSetting.RequiredSpecializationType)
                .Select(x => new AttributeSetting(x)).ToList();
            OperationSettings = element.ChildElements
                .Where(x => x.SpecializationType == OperationSetting.RequiredSpecializationType)
                .Select(x => new OperationSetting(x)).ToList();
            ChildElementSettings = element.ChildElements
                .Where(x => x.SpecializationType == ElementSetting.RequiredSpecializationType)
                .Select(x => new ElementSetting(x)).ToList();
            CreationOptions = element.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new CreationOption(x)).ToList();
            TypeOrder = element.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new TypeOrder(x)).ToList();
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

        public IList<LiteralSetting> LiteralSettings { get; set; }
        public IList<AttributeSetting> AttributeSettings { get; set; }
        public IList<OperationSetting> OperationSettings { get; set; }
        public IList<ElementSetting> ChildElementSettings { get; set; }
        public IList<CreationOption> CreationOptions { get; set; }
    }
}