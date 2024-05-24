using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationDestinationEndSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Association Destination End Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public AssociationDestinationEndSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public bool TargetsType(string elementSettingsId)
        {
            return this.GetSettings().TargetTypes().Any(t => t.Id == elementSettingsId);
        }

        public List<IElement> TargetTypes()
        {
            return this.GetSettings().TargetTypes().ToList();
        }

        public string ApiModelName => _element.GetApiModelName();

        public string ApiPropertyName => this.GetSettings().ApiPropertyName() ?? Name.RemoveSuffix("End").ToCSharpIdentifier().Pluralize(true);

        public AssociationEndSettingsPersistable ToPersistable()
        {
            return new AssociationEndSettingsPersistable
            {
                SpecializationTypeId = this.Id,
                SpecializationType = this.Name,
                DisplayFunction = this.GetSettings().DisplayTextFunction(),
                NameAccessibilityMode = Enum.Parse<FieldAccessibilityMode>(this.GetSettings().NameAccessibility().Value),
                DefaultNameFunction = this.GetSettings().DefaultNameFunction(),
                NameMustBeUnique = this.GetSettings().NameMustBeUnique(),
                Icon = this.GetSettings().Icon().ToPersistable(),
                TypeReferenceSetting = new TypeReferenceSettingPersistable()
                {
                    TargetTypes = (this.GetSettings().TargetTypes()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .Concat(this.GetSettings().TargetTraits()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .ToArray(),
                    IsCollectionDefault = this.GetSettings().IsCollectionDefault(),
                    AllowIsCollection = this.GetSettings().IsCollectionEnabled(),
                    IsNavigableDefault = this.GetSettings().IsNavigableDefault(),
                    AllowIsNavigable = this.GetSettings().IsNavigableEnabled(),
                    IsNullableDefault = this.GetSettings().IsNullableDefault(),
                    AllowIsNullable = this.GetSettings().IsNullableEnabled()
                },
                AllowSorting = this.GetSettings().AllowSorting(),
                SortChildren = ToSortChildrenOptions(this.GetSettings().SortChildren()),
                TypeOrder = this.MenuOptions?.TypeOrder.Select(x => x.ToPersistable()).ToList(),
                CreationOptions = this.MenuOptions?.ToCreationOptionsPersistable(),
                ScriptOptions = MenuOptions?.RunScriptOptions.Select(x => x.ToPersistable()).ToList(),
                MappingOptions = MenuOptions?.MappingOptions.Select(x => x.ToPersistable()).ToList()
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationDestinationEndSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationDestinationEndSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public const string SpecializationTypeId = "c4c61fdc-464d-41d2-8e0e-5a734d588302";

        public string Comment => _element.Comment;

        private static SortChildrenOptions? ToSortChildrenOptions(AssociationDestinationEndSettingsModelStereotypeExtensions.Settings.SortChildrenOptions options)
        {
            if (options == null)
            {
                return null;
            }

            if (options.IsManually())
            {
                return SortChildrenOptions.Manually;
            }
            if (options.IsByTypeThenManually())
            {
                return SortChildrenOptions.SortByTypeThenManually;
            }
            if (options.IsByTypeThenName())
            {
                return SortChildrenOptions.SortByTypeAndName;
            }
            if (options.IsByName())
            {
                return SortChildrenOptions.SortByName;
            }

            return null;
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationDestinationEndSettingsModelExtensions
    {

        public static bool IsAssociationDestinationEndSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationDestinationEndSettingsModel.SpecializationTypeId;
        }

        public static AssociationDestinationEndSettingsModel AsAssociationDestinationEndSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationDestinationEndSettingsModel() ? new AssociationDestinationEndSettingsModel((IElement)type) : null;
        }
    }
}