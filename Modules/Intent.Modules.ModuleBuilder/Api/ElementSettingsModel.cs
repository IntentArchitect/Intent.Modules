using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api.Factories;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ElementSettingsModel : IHasStereotypes, IMetadataModel, ICreatableType, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Element Settings";
        public const string RequiredSpecializationType = "Element Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]

        public ElementSettingsModel(IElement element, string requiredType = SpecializationType)
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
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public string ApiModelName => $"{Name.ToCSharpIdentifier()}Model";

        public DesignerSettingsModel DesignerSettings => DesignerModelFactory.GetDesignerSettings(_element);

        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);

        public bool IsChild => _element.ParentElement.SpecializationType == SpecializationType;

        public ElementSettingPersistable ToPersistable()
        {
            return new ElementSettingPersistable()
            {
                SpecializationTypeId = this.Id,
                SpecializationType = this.Name,
                Implements = this.Stereotypes
                    .Where(x => x.DefinitionId != ElementSettingsModelStereotypeExtensions.Settings.DefinitionId &&
                                x.DefinitionId != ElementSettingsModelStereotypeExtensions.TypeReferenceSettings.DefinitionId)
                    .Select(x => new ImplementedTraitPersistable() { Id = x.DefinitionId, Name = x.Name })
                    .ToList(),
                SaveAsOwnFile = MustSaveInOwnFile(),
                DisplayFunction = this.GetSettings().DisplayTextFunction(),
                ValidateFunction = this.GetSettings().ValidateFunction(),
                Icon = GetIcon(this.GetSettings().Icon()) ?? new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" },
                ExpandedIcon = GetIcon(this.GetSettings().ExpandedIcon()),
                IconFunction = this.GetSettings().IconFunction(),
                AllowRename = this.GetSettings().AllowRename(),
                AllowAbstract = this.GetSettings().AllowAbstract(),
                AllowStatic = this.GetSettings().AllowStatic(),
                AllowSetValue = this.GetSettings().AllowSetValue(),
                AllowGenericTypes = this.GetSettings().AllowGenericTypes(),
                AllowMapping = this.MappingSettings.Any(),
                AllowSorting = this.GetSettings().AllowSorting(),
                SortChildren = ToSortChildrenOptions(this.GetSettings().SortChildren()),
                AllowFindInView = this.GetSettings().AllowFindInView(),
                AllowTypeReference = !this.GetTypeReferenceSettings().Mode().IsDisabled(),
                TypeReferenceSetting = !this.GetTypeReferenceSettings().Mode().IsDisabled() ? new TypeReferenceSettingPersistable()
                {
                    IsRequired = this.GetTypeReferenceSettings().Mode().IsRequired(),
                    TargetTypes = (this.GetTypeReferenceSettings().TargetTypes()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .Concat(this.GetTypeReferenceSettings().TargetTraits()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .ToArray(),
                    AllowIsNullable = this.GetTypeReferenceSettings().AllowNullable(),
                    AllowIsCollection = this.GetTypeReferenceSettings().AllowCollection(),
                    DefaultTypeId = this.GetTypeReferenceSettings().DefaultTypeId(),
                    DisplayName = this.GetTypeReferenceSettings().DisplayName(),
                    Hint = this.GetTypeReferenceSettings().Hint()
                } : null,
                DiagramSettings = DiagramSettings?.ToPersistable(),
                ChildElementSettings = this.ElementSettings.Select(x => x.ToPersistable()).ToArray(),
                MappingSettings = this.MappingSettings.Select(x => x.ToPersistable()).ToList(),
                CreationOptions = this.MenuOptions?.ToCreationOptionsPersistable(),
                ScriptOptions = MenuOptions?.RunScriptOptions.Select(x => x.ToPersistable()).ToList(),
                MappingOptions = MenuOptions?.MappingOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = GetTypeOrder(),
                VisualSettings = this.VisualSettings?.ToPersistable(),
                Macros = EventSettings?.ToPersistable(),
            };
        }

        private List<TypeOrderPersistable> GetTypeOrder()
        {
            var result = AcceptedChildTypes?.ToPersistable() ?? new List<TypeOrderPersistable>();
            result = result
                .Concat(MenuOptions?.TypeOrder
                    .Select((x) => x.ToPersistable())
                    .Where(x => !result.Contains(x)) ?? new List<TypeOrderPersistable>())
                .ToList();
            return result;
        }

        private IconModelPersistable GetIcon(IIconModel icon)
        {
            return icon != null ? new IconModelPersistable { Type = (IconType)icon.Type, Source = icon.Source } : null;
        }

        public ElementSettingsModel GetInheritedType()
        {
            return !this.GetTypeReferenceSettings().Mode().IsDisabled() &&
                   this.GetTypeReferenceSettings().Represents().IsInheritance()
                ? new ElementSettingsModel(this.GetTypeReferenceSettings().TargetTypes().Single())
                : null;
        }

        private static SortChildrenOptions? ToSortChildrenOptions(
            ElementSettingsModelStereotypeExtensions.Settings.SortChildrenOptions options)
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

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementSettings => _element.ChildElements
            .GetElementsOfType(ElementSettingsModel.SpecializationTypeId)
            .Select(x => new ElementSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public DiagramSettingsModel DiagramSettings => _element.ChildElements
            .GetElementsOfType(DiagramSettingsModel.SpecializationTypeId)
            .Select(x => new DiagramSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<MappingProjectionSettingsModel> MappingSettings => _element.ChildElements
            .GetElementsOfType(MappingProjectionSettingsModel.SpecializationTypeId)
            .Select(x => new MappingProjectionSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        public AcceptedChildTypesModel AcceptedChildTypes => _element.ChildElements
            .GetElementsOfType(AcceptedChildTypesModel.SpecializationTypeId)
            .Select(x => new AcceptedChildTypesModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public ElementVisualSettingsModel VisualSettings => _element.ChildElements
            .GetElementsOfType(ElementVisualSettingsModel.SpecializationTypeId)
            .Select(x => new ElementVisualSettingsModel(x))
            .SingleOrDefault();

        public bool MustSaveInOwnFile()
        {
            if (this.GetSettings().SaveMode().IsDefault())
            {
                return _element.ParentElement.SpecializationType != ElementSettingsModel.SpecializationType;
            }
            return this.GetSettings().SaveMode().IsOwnFile();
        }

        [IntentManaged(Mode.Fully)]
        public ElementEventSettingsModel EventSettings => _element.ChildElements
            .GetElementsOfType(ElementEventSettingsModel.SpecializationTypeId)
            .Select(x => new ElementEventSettingsModel(x))
            .SingleOrDefault();
        public const string SpecializationTypeId = "727577aa-3e07-4b41-be7d-7359bb1e48c8";

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class ElementSettingsModelExtensions
    {

        public static bool IsElementSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ElementSettingsModel.SpecializationTypeId;
        }

        public static ElementSettingsModel AsElementSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsElementSettingsModel() ? new ElementSettingsModel((IElement)type) : null;
        }
    }
}