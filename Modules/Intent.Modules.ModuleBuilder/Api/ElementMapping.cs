using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementMapping
    {
        public const string SpecializationType = "Element Mapping";
        private readonly IElement _element;

        public ElementMapping(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'ElementMapping' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public MappingCriteria Criteria => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingCriteria.SpecializationType)
            .Select(x => new MappingCriteria(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public MappingOutput Output => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingOutput.SpecializationType)
            .Select(x => new MappingOutput(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public IList<ElementMapping> ChildMappings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementMapping.SpecializationType)
            .Select(x => new ElementMapping(x))
            .ToList<ElementMapping>();

        protected bool Equals(ElementMapping other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementMapping)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}