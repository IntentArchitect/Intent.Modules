using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DesignerSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Designer Settings";

        [IntentManaged(Mode.Ignore)]
        public DesignerSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{nameof(DesignerSettingsModel)}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            //ElementSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.ElementSettings.RequiredSpecializationType)
            //    .Select(x => new ElementSettings(x)).OrderBy(x => x.Name)
            //    .ToList<IElementSettings>();
            //AssociationSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == AssociationSetting.RequiredSpecializationType)
            //    .Select(x => new AssociationSetting(x)).OrderBy(x => x.SpecializationType)
            //    .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentIgnore]
        public virtual string MetadataName()
        {
            return Name;
        }

        [IntentManaged(Mode.Fully)]
        public IList<AssociationSettingsModel> AssociationTypes => _element.ChildElements
            .GetElementsOfType(AssociationSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationSettingsModel(x))
            .ToList();

        public IList<AssociationExtensionModel> AssociationExtensions => _element.ChildElements
            .GetElementsOfType(AssociationExtensionModel.SpecializationTypeId)
            .Select(x => new AssociationExtensionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementTypes => _element.ChildElements
            .GetElementsOfType(ElementSettingsModel.SpecializationTypeId)
            .Select(x => new ElementSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(DesignerSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignerSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<CoreTypeModel> CoreTypes => _element.ChildElements
            .GetElementsOfType(CoreTypeModel.SpecializationTypeId)
            .Select(x => new CoreTypeModel(x))
            .ToList();

        [IntentIgnore]
        public virtual bool IsReference()
        {
            return this.GetDesignerSettings().IsReference();
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IList<ScriptModel> ScriptTypes => _element.ChildElements
            .GetElementsOfType(ScriptModel.SpecializationTypeId)
            .Select(x => new ScriptModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .GetElementsOfType(ElementExtensionModel.SpecializationTypeId)
            .Select(x => new ElementExtensionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<PackageExtensionModel> PackageExtensions => _element.ChildElements
            .GetElementsOfType(PackageExtensionModel.SpecializationTypeId)
            .Select(x => new PackageExtensionModel(x))
            .ToList();

        public IList<MappingSettingsModel> MappingSettings => _element.ChildElements
            .GetElementsOfType(MappingSettingsModel.SpecializationTypeId)
            .Select(x => new MappingSettingsModel(x))
            .ToList();

        public IList<MappableElementsPackageModel> MappableElementsPackages => _element.ChildElements
            .GetElementsOfType(MappableElementsPackageModel.SpecializationTypeId)
            .Select(x => new MappableElementsPackageModel(x))
            .ToList();

        public IList<MappableElementsPackageExtensionModel> MappableElementsPackageExtensions => _element.ChildElements
            .GetElementsOfType(MappableElementsPackageExtensionModel.SpecializationTypeId)
            .Select(x => new MappableElementsPackageExtensionModel(x))
            .ToList();

        public SuggestionsSettingsModel Suggestions => _element.ChildElements
            .GetElementsOfType(SuggestionsSettingsModel.SpecializationTypeId)
            .Select(x => new SuggestionsSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<PackageSettingsModel> PackageTypes => _element.ChildElements
            .GetElementsOfType(PackageSettingsModel.SpecializationTypeId)
            .Select(x => new PackageSettingsModel(x))
            .ToList();
        public const string SpecializationTypeId = "7a6411a8-ffef-4209-91c6-8d12755a806a";

        public IList<DesignerReferenceModel> DesignerReferences => _element.ChildElements
            .GetElementsOfType(DesignerReferenceModel.SpecializationTypeId)
            .Select(x => new DesignerReferenceModel(x))
            .ToList();

        public string Comment => _element.Comment;

        [IntentIgnore]
        public DesignerSettingsPersistable ToPersistable()
        {
            var modelerSettings = new DesignerSettingsPersistable
            {
                Id = Id,
                Name = Name,
                DesignerReferences = DesignerReferences.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                PackageSettings = PackageTypes.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                PackageExtensions = PackageExtensions.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                ElementSettings = ElementTypes.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                ElementExtensions = ElementExtensions.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                AssociationSettings = AssociationTypes.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                AssociationExtensions = AssociationExtensions.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                MappingSettings = MappingSettings.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                MappableElementPackages = MappableElementsPackages.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                MappableElementPackageExtensions = MappableElementsPackageExtensions.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList(),
                SuggestionSettings = Suggestions?.Suggestions.OrderBy(x => x.TypeReference.Element.Name).ThenBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList() ?? [],
                Scripts = ScriptTypes.OrderBy(x => x.Name).ThenBy(x => x.Id).Select(x => x.ToPersistable()).ToList()
            };
            return modelerSettings;
        }
    }

    public class TypeOrder : IEquatable<TypeOrder>
    {
        public TypeOrder(ElementCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.Name;
            TypeId = element.TypeReference.Element.Id;
        }

        public TypeOrder(StereotypeDefinitionCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.Name;
            TypeId = element.TypeReference.Element.Id;
        }

        public TypeOrder(AssociationCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.IsAssociationSettingsModel() ? element.TypeReference.Element.AsAssociationSettingsModel().TargetEnd.Name : element.TypeReference.Element.Name;
            TypeId = element.TypeReference.Element.IsAssociationSettingsModel() ? element.TypeReference.Element.AsAssociationSettingsModel().TargetEnd.Id : element.TypeReference.Element.Id;
        }

        public int? Order { get; set; }
        public string Type { get; set; }
        public string TypeId { get; set; }

        public bool Equals(TypeOrder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(TypeId, other.TypeId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeOrder)obj);
        }

        public override int GetHashCode()
        {
            return (TypeId != null ? TypeId.GetHashCode() : 0);
        }

        public TypeOrderPersistable ToPersistable()
        {
            return new TypeOrderPersistable { Type = Type, TypeId = TypeId, Order = Order };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DesignerSettingsModelExtensions
    {

        public static bool IsDesignerSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DesignerSettingsModel.SpecializationTypeId;
        }

        public static DesignerSettingsModel AsDesignerSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsDesignerSettingsModel() ? new DesignerSettingsModel((IElement)type) : null;
        }
    }
}
