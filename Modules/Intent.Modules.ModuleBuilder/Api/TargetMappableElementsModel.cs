using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class TargetMappableElementsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Target Mappable Elements";
        public const string SpecializationTypeId = "a812bff7-017a-4692-8ec7-1caad0a1dd88";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public TargetMappableElementsModel(IElement element, string requiredType = SpecializationType)
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

        public IList<MappableElementSettingsModel> MappableElements => _element.ChildElements
            .GetElementsOfType(MappableElementSettingsModel.SpecializationTypeId)
            .Select(x => new MappableElementSettingsModel(x))
            .ToList();

        public IList<MappableElementsPackageImportModel> ImportMappableElementsPackages => _element.ChildElements
            .GetElementsOfType(MappableElementsPackageImportModel.SpecializationTypeId)
            .Select(x => new MappableElementsPackageImportModel(x))
            .ToList();

        [IntentIgnore]
        public List<object> GetMappableElementPersistables()
        {
            return _element.ChildElements.Select(x =>
            {
                if (x.IsMappableElementSettingsModel()) return (object)x.AsMappableElementSettingsModel().ToPersistable();
                if (x.IsMappableElementsPackageImportModel()) return (object)x.AsMappableElementsPackageImportModel().ToPersistable();
                return null;
            }).Where(x => x is not null).ToList();
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(TargetMappableElementsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TargetMappableElementsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class TargetMappableElementsModelExtensions
    {

        public static bool IsTargetMappableElementsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TargetMappableElementsModel.SpecializationTypeId;
        }

        public static TargetMappableElementsModel AsTargetMappableElementsModel(this ICanBeReferencedType type)
        {
            return type.IsTargetMappableElementsModel() ? new TargetMappableElementsModel((IElement)type) : null;
        }
    }
}