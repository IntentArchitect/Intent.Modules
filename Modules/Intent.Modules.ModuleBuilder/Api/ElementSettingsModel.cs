using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api.Factories;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class ElementSettingsModel : IHasStereotypes, IMetadataModel, ICreatableType, IHasName
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
                SaveAsOwnFile = MustSaveInOwnFile(),
                DisplayFunction = this.GetSettings().DisplayTextFunction(),
                Icon = GetIcon(this.GetSettings().Icon()) ?? new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" },
                ExpandedIcon = GetIcon(this.GetSettings().ExpandedIcon()),
                AllowRename = this.GetSettings().AllowRename(),
                AllowAbstract = this.GetSettings().AllowAbstract(),
                AllowSetValue = this.GetSettings().AllowSetValue(),
                AllowGenericTypes = this.GetSettings().AllowGenericTypes(),
                AllowMapping = this.MappingSettings != null,
                AllowSorting = this.GetSettings().AllowSorting(),
                SortChildren = ToSortChildrenOptions(this.GetSettings().SortChildren()),
                AllowFindInView = this.GetSettings().AllowFindInView(),
                AllowTypeReference = !this.GetTypeReferenceSettings().Mode().IsDisabled(),
                TypeReferenceSetting = !this.GetTypeReferenceSettings().Mode().IsDisabled() ? new TypeReferenceSettingPersistable()
                {
                    IsRequired = this.GetTypeReferenceSettings().Mode().IsRequired(),
                    TargetTypes = this.GetTypeReferenceSettings().TargetTypes()?.Select(e => e.Name).ToArray(),
                    AllowIsNullable = this.GetTypeReferenceSettings().AllowNullable(),
                    AllowIsCollection = this.GetTypeReferenceSettings().AllowCollection(),
                    DefaultTypeId = this.GetTypeReferenceSettings().DefaultTypeId()
                } : null,
                DiagramSettings = DiagramSettings?.ToPersistable(),
                ChildElementSettings = this.ElementSettings.Select(x => x.ToPersistable()).ToArray(),
                MappingSettings = this.MappingSettings?.ToPersistable(),
                CreationOptions = this.MenuOptions?.ElementCreations.Select(x => x.ToPersistable())
                    .Concat(this.MenuOptions.AssociationCreations.Select(x => x.ToPersistable()))
                    .Concat(MenuOptions.StereotypeDefinitionCreation != null ? new[] { MenuOptions.StereotypeDefinitionCreation.ToPersistable() } : new ElementCreationOption[0])
                    .ToList(),
                TypeOrder = this.MenuOptions?.TypeOrder.Select((t, index) => new TypeOrderPersistable { Type = t.Type, Order = t.Order?.ToString() }).ToList(),
                VisualSettings = this.VisualSettings?.ToPersistable(),
                Macros = this.EventSettings?.ToPersistable()
            };
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
            ElementSettingsModelExtensions.Settings.SortChildrenOptions options)
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
        public MappingSettingsModel MappingSettings => _element.ChildElements
            .GetElementsOfType(MappingSettingsModel.SpecializationTypeId)
            .Select(x => new MappingSettingsModel(x))
            .SingleOrDefault();

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
}