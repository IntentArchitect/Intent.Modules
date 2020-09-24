using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ComponentCommandModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Component Command";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ComponentCommandModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference ReturnType => TypeReference.Element != null ? TypeReference : null;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ComponentCommandModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentCommandModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }


        [IntentManaged(Mode.Fully)]
        public IList<ParameterModel> Parameters => _element.ChildElements
            .Where(x => x.SpecializationType == ParameterModel.SpecializationType)
            .Select(x => new ParameterModel(x))
            .ToList();
        public const string SpecializationTypeId = "d6739ffc-30e6-4170-a105-bf28e69aa578";
    }
}