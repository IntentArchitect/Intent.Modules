using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MappingSettings
    {
        public const string SpecializationType = "Mapping Settings";
        private readonly IElement _element;

        public MappingSettings(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        protected bool Equals(MappingSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<ElementMapping> Mappings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementMapping.SpecializationType)
            .Select(x => new ElementMapping(x))
            .ToList<ElementMapping>();

        public ElementMappingSettings ToPersistable()
        {
            throw new NotImplementedException();
        }
    }
}