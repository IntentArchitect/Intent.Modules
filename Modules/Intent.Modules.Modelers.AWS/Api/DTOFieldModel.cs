using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DTOFieldModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "DTO-Field";
        public const string SpecializationTypeId = "9e1a18f3-c6b1-495e-81be-cea88f37706f";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DTOFieldModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public string Value => _element.Value;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DTOFieldModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DTOFieldModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DTOFieldModelExtensions
    {

        public static bool IsDTOFieldModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DTOFieldModel.SpecializationTypeId;
        }

        public static DTOFieldModel AsDTOFieldModel(this ICanBeReferencedType type)
        {
            return type.IsDTOFieldModel() ? new DTOFieldModel((IElement)type) : null;
        }
    }
}