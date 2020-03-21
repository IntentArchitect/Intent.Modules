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
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

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