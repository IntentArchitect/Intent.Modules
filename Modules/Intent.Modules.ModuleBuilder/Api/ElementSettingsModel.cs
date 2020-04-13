using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementSettingsModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Element Settings";
        public const string RequiredSpecializationType = "Element Settings";
        protected readonly IElement _element;


        public ElementSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

        public DesignerModel Designer => new DesignerModel(_element.GetParentPath().Single(x => x.SpecializationType == Api.DesignerModel.SpecializationType));

        public bool IsChild => _element.ParentElement.SpecializationType == SpecializationType;

        public ElementSettingsPersistable ToPersistable()
        {
            return new ElementSettingsPersistable()
            {
                SpecializationType = this.Name,
                SaveAsOwnFile = this.GetSettings().SaveMode().IsOwnFile(),
                DisplayFunction = this.GetSettings().DisplayTextFunction(),
                Icon = GetIcon(this.GetIconFull()) ?? new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" },
                ExpandedIcon = GetIcon(this.GetIconFullExpanded()),
                AllowRename = this.GetSettings().AllowRename(),
                AllowAbstract = this.GetSettings().AllowAbstract(),
                AllowGenericTypes = this.GetSettings().AllowGenericTypes(),
                AllowMapping = this.GetSettings().AllowMapping(),
                AllowSorting = this.GetSettings().AllowSorting(),
                AllowFindInView = this.GetSettings().AllowFindInView(),
                AllowTypeReference = !this.GetTypeReferenceSettings().Mode().IsDisabled(),
                TypeReferenceSetting = !this.GetTypeReferenceSettings().Mode().IsDisabled() ? new TypeReferenceSettingPersistable()
                {
                    IsRequired = this.GetTypeReferenceSettings().Mode().IsRequired(),
                    TargetTypes = this.GetTypeReferenceSettings().TargetTypes()?.Select(e => e.Name).ToArray(),
                    AllowIsNullable = this.GetTypeReferenceSettings().AllowNullable(),
                    AllowIsCollection = this.GetTypeReferenceSettings().AllowCollection(),
                } : null,
                DiagramSettings = null, // TODO JL / GCB
                ChildElementSettings = this.ElementSettings.Select(x => x.ToPersistable()).ToArray(),
                MappingSettings = this.MappingSettings?.ToPersistable(),
                CreationOptions = this.MenuOptions?.CreationOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = this.MenuOptions?.TypeOrder.Select((t, index) => new TypeOrderPersistable { Type = t.Type, Order = t.Order?.ToString() }).ToList()
            };
        }


        private IconModelPersistable GetIcon(ElementSettingsExtensions.IconFullExpanded icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        private IconModelPersistable GetIcon(ElementSettingsExtensions.IconFull icon)
        {
            return icon != null ? new IconModelPersistable { Type = Enum.Parse<IconType>(icon.Type().Value), Source = icon.Source() } : null;
        }

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ContextMenuModel.SpecializationType)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettingsModel.SpecializationType)
            .Select(x => new ElementSettingsModel(x))
            .ToList<ElementSettingsModel>();

        [IntentManaged(Mode.Fully)]
        public IList<DiagramSettingsModel> DiagramSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DiagramSettingsModel.SpecializationType)
            .Select(x => new DiagramSettingsModel(x))
            .ToList<DiagramSettingsModel>();

        [IntentManaged(Mode.Fully)]
        public MappingSettingsModel MappingSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingSettingsModel.SpecializationType)
            .Select(x => new MappingSettingsModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        protected bool Equals(ElementSettingsModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}