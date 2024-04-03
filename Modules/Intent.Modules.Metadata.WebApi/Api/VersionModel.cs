using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class VersionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Version";
        public const string SpecializationTypeId = "95f4362b-8193-41cb-bd65-c17d6be3f96b";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public VersionModel(IElement element, string requiredType = SpecializationType)
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

        [IntentManaged(Mode.Ignore)]
        public ApiVersionModel ApiVersion => new(_element.ParentElement);

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(VersionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class VersionModelExtensions
    {

        public static bool IsVersionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == VersionModel.SpecializationTypeId;
        }

        public static VersionModel AsVersionModel(this ICanBeReferencedType type)
        {
            return type.IsVersionModel() ? new VersionModel((IElement)type) : null;
        }
    }
}