using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class EntityModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Entity";
        public const string SpecializationTypeId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public EntityModel(IElement element, string requiredType = SpecializationType)
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

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public IElement InternalElement => _element;

        public IList<AttributeModel> Attributes => _element.ChildElements
            .GetElementsOfType(AttributeModel.SpecializationTypeId)
            .Select(x => new AttributeModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(EntityModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EntityModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class EntityModelExtensions
    {

        public static bool IsEntityModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == EntityModel.SpecializationTypeId;
        }

        public static EntityModel AsEntityModel(this ICanBeReferencedType type)
        {
            return type.IsEntityModel() ? new EntityModel((IElement)type) : null;
        }
    }
}