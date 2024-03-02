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
    public class ResourceModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Resource";
        public const string SpecializationTypeId = "dca1ab24-80eb-42d2-9c8b-67803ba85ca2";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ResourceModel(IElement element, string requiredType = SpecializationType)
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

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ResourceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResourceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ResourceModelExtensions
    {

        public static bool IsResourceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ResourceModel.SpecializationTypeId;
        }

        public static ResourceModel AsResourceModel(this ICanBeReferencedType type)
        {
            return type.IsResourceModel() ? new ResourceModel((IElement)type) : null;
        }
    }
}