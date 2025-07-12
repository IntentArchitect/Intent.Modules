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
    public class MappableElementsPackageModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Mappable Elements Package";
        public const string SpecializationTypeId = "aa2eab42-5ffc-4028-b4b6-95ff719705d4";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementsPackageModel(IElement element, string requiredType = SpecializationTypeId)
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

        public bool Equals(MappableElementsPackageModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementsPackageModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementsPackagePersistable ToPersistable()
        {
            return new MappableElementsPackagePersistable
            {
                Id = Id,
                Name = Name,
                MappableElements = MappableElements.Select(x => x.ToPersistable()).ToList(),
                PackageImports = ImportMappableElementsPackages.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementsPackageModelExtensions
    {

        public static bool IsMappableElementsPackageModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementsPackageModel.SpecializationTypeId;
        }

        public static MappableElementsPackageModel AsMappableElementsPackageModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementsPackageModel() ? new MappableElementsPackageModel((IElement)type) : null;
        }
    }
}