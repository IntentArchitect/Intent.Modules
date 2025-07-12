using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappableElementsPackageImportModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Mappable Elements Package Import";
        public const string SpecializationTypeId = "90892fa1-b0b9-4af9-9f3b-4fdf37db9b05";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementsPackageImportModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference ReferenceSettings => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappableElementsPackageImportModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementsPackageImportModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementsPackageImportPersistable ToPersistable()
        {
            return new MappableElementsPackageImportPersistable
            {
                Id = TypeReference.Element.Id,
                Name = TypeReference.Element.Name,
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementsPackageImportModelExtensions
    {

        public static bool IsMappableElementsPackageImportModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementsPackageImportModel.SpecializationTypeId;
        }

        public static MappableElementsPackageImportModel AsMappableElementsPackageImportModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementsPackageImportModel() ? new MappableElementsPackageImportModel((IElement)type) : null;
        }
    }
}