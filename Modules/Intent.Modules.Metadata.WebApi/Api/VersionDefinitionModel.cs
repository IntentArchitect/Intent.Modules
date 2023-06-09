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
    public class VersionDefinitionModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Version Definition";
        public const string SpecializationTypeId = "0ec760ae-98d8-4e8e-953c-d00691ff7e28";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public VersionDefinitionModel(IElement element, string requiredType = SpecializationType)
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

        public IList<VersionModel> Versions => _element.ChildElements
            .GetElementsOfType(VersionModel.SpecializationTypeId)
            .Select(x => new VersionModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(VersionDefinitionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersionDefinitionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class VersionDefinitionModelExtensions
    {

        public static bool IsVersionDefinitionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == VersionDefinitionModel.SpecializationTypeId;
        }

        public static VersionDefinitionModel AsVersionDefinitionModel(this ICanBeReferencedType type)
        {
            return type.IsVersionDefinitionModel() ? new VersionDefinitionModel((IElement)type) : null;
        }
    }
}