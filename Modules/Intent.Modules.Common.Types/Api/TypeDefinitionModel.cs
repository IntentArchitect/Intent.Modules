using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Common.Types.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TypeDefinitionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Type-Definition";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public TypeDefinitionModel(IElement element, string requiredType = SpecializationType)
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
        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        public IList<AttributeModel> Attributes => _element.ChildElements
            .GetElementsOfType(AttributeModel.SpecializationTypeId)
            .Select(x => new AttributeModel(x))
            .ToList();

        public IList<OperationModel> Operations => _element.ChildElements
            .GetElementsOfType(OperationModel.SpecializationTypeId)
            .Select(x => new OperationModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(TypeDefinitionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeDefinitionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e";

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class TypeDefinitionModelExtensions
    {

        public static bool IsTypeDefinitionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TypeDefinitionModel.SpecializationTypeId;
        }

        public static TypeDefinitionModel AsTypeDefinitionModel(this ICanBeReferencedType type)
        {
            return type.IsTypeDefinitionModel() ? new TypeDefinitionModel((IElement)type) : null;
        }
    }
}