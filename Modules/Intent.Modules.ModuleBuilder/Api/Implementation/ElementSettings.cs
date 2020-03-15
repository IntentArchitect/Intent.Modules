using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class ElementSettings : IElementSettings
    {
        public const string SpecializationType = "Element Settings";
        public const string RequiredSpecializationType = "Element Settings";
        private readonly IElement _element;


        public ElementSettings(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

            Icon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full)"));
            ExpandedIcon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full, Expanded)"));
            AllowRename = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
            AllowAbstract = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Abstract");
            AllowGenericTypes = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Generic Types");
            AllowMapping = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Mapping");
            AllowSorting = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Sorting");
            AllowFindInView = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
            //LiteralSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.LiteralSettings.SpecializationType)
            //    .Select(x => new LiteralSettings(x))
            //    .ToList<ILiteralSettings>();
            //AttributeSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.AttributeSettings.SpecializationType)
            //    .Select(x => new AttributeSettings(x))
            //    .ToList<IAttributeSettings>();
            //OperationSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.OperationSettings.SpecializationType)
            //    .Select(x => new OperationSettings(x))
            //    .ToList<IOperationSettings>();
            //ChildElementSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.ElementSettings.SpecializationType)
            //    .Select(x => new ElementSettings(x))
            //    .ToList<IElementSettings>();
            ////CreationOptions = element.ChildElements.SingleOrDefault(x => x.SpecializationType == "Creation Options")?.Attributes.Select(x => new CreationOption(x)).ToList();
            //MenuOptions = element.ChildElements.Any(x => x.SpecializationType == Api.ContextMenu.SpecializationType) ? new ContextMenu(element.ChildElements.Single(x => x.SpecializationType == Api.ContextMenu.SpecializationType)) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

        public IconModel Icon { get; set; }

        public IconModel ExpandedIcon { get; set; }

        public bool? AllowRename { get; set; }

        public bool? AllowAbstract { get; set; }

        public bool? AllowGenericTypes { get; set; }

        public bool? AllowMapping { get; set; }

        public bool? AllowSorting { get; set; }

        public bool? AllowFindInView { get; set; }

        [IntentManaged(Mode.Fully)]
        public IContextMenu MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ContextMenu.SpecializationType)
            .Select(x => new ContextMenu(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<ILiteralSettings> LiteralSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.LiteralSettings.SpecializationType)
            .Select(x => new LiteralSettings(x))
            .ToList<ILiteralSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IAttributeSettings> AttributeSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AttributeSettings.SpecializationType)
            .Select(x => new AttributeSettings(x))
            .ToList<IAttributeSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IOperationSettings> OperationSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.OperationSettings.SpecializationType)
            .Select(x => new OperationSettings(x))
            .ToList<IOperationSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IElementSettings> ChildElementSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettings.SpecializationType)
            .Select(x => new ElementSettings(x))
            .ToList<IElementSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IDiagramSettings> DiagramSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DiagramSettings.SpecializationType)
            .Select(x => new DiagramSettings(x))
            .ToList<IDiagramSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IMappingSettings> MappingSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingSettings.SpecializationType)
            .Select(x => new MappingSettings(x))
            .ToList<IMappingSettings>();

        public override string ToString()
        {
            return _element.ToString();
        }

        protected bool Equals(ElementSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}