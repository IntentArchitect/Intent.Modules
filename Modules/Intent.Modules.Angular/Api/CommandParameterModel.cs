using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge)]
    public class CommandParameterModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Command Parameter";
        public const string SpecializationTypeId = "4a1ce2a9-e79f-4e93-96af-24b44728ecbc";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public CommandParameterModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CommandParameterModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CommandParameterModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}