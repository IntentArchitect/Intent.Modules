using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    public class ElementSettings : IElementSettings
    {
        private readonly IElement _element;
        public const string RequiredSpecializationType = "Element Settings";

        public ElementSettings(IElement element) 
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

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
                .Where(x => x.SpecializationType == Api.LiteralSettings.RequiredSpecializationType)
                .Select(x => new LiteralSettings(x)).ToList();
            AttributeSettings = element.ChildElements
                .Where(x => x.SpecializationType == Api.AttributeSettings.RequiredSpecializationType)
                .Select(x => new AttributeSettings(x)).ToList();
            OperationSettings = element.ChildElements
                .Where(x => x.SpecializationType == OperationSetting.RequiredSpecializationType)
                .Select(x => new OperationSetting(x)).ToList();
            ChildElementSettings = element.ChildElements
                .Where(x => x.SpecializationType == ElementSettings.RequiredSpecializationType)
                .Select(x => new ElementSettings(x)).ToList();
            //CreationOptions = element.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new CreationOption(x)).ToList();
            ContextMenu = element.ChildElements.Any(x => x.SpecializationType == Api.ContextMenu.SpecializationType) ? new ContextMenu(element.ChildElements.Single(x => x.SpecializationType == "Creation Options")) : null;
            TypeOrder = element.ChildElements.SingleOrDefault(x => x.SpecializationType == Api.ContextMenu.SpecializationType)?.Attributes.Select(x => new TypeOrder(x)).ToList();
        }


        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;
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

        public IList<LiteralSettings> LiteralSettings { get; set; }
        public IList<AttributeSettings> AttributeSettings { get; set; }
        public IList<OperationSetting> OperationSettings { get; set; }
        public IList<ElementSettings> ChildElementSettings { get; set; }
        public IContextMenu ContextMenu { get; set; }

        public override string ToString()
        {
            return _element.ToString();
        }
    }
}