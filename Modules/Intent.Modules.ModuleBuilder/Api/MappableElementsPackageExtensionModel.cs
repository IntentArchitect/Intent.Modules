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
    public class MappableElementsPackageExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Mappable Elements Package Extension";
        public const string SpecializationTypeId = "bddb69f0-e2c8-4373-b5cb-d701f311f935";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementsPackageExtensionModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference Extends => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<MappableElementsPackageImportModel> ImportMappableElementsPackages => _element.ChildElements
            .GetElementsOfType(MappableElementsPackageImportModel.SpecializationTypeId)
            .Select(x => new MappableElementsPackageImportModel(x))
            .ToList();

        public IList<MappableElementSettingsModel> MappableElements => _element.ChildElements
            .GetElementsOfType(MappableElementSettingsModel.SpecializationTypeId)
            .Select(x => new MappableElementSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappableElementsPackageExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementsPackageExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementsPackageExtensionPersistable ToPersistable()
        {
            return new MappableElementsPackageExtensionPersistable
            {
                Id = Id,
                Name = Name,
                ExtendPackageId = TypeReference.Element.Id,
                ExtendPackage = TypeReference.Element.Name,
                MappableElements = MappableElements.Select(x => x.ToPersistable()).ToList(),
                PackageImports = ImportMappableElementsPackages.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementsPackageExtensionModelExtensions
    {

        public static bool IsMappableElementsPackageExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementsPackageExtensionModel.SpecializationTypeId;
        }

        public static MappableElementsPackageExtensionModel AsMappableElementsPackageExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementsPackageExtensionModel() ? new MappableElementsPackageExtensionModel((IElement)type) : null;
        }
    }
}