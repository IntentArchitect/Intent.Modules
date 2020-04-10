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
    internal class ElementSettings : IElementSettings
    {
        public const string SpecializationType = "Element Settings";
        public const string RequiredSpecializationType = "Element Settings";
        private readonly IElement _element;


        public ElementSettings(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

        public IModeler Modeler => new Modeler(_element.GetParentPath().Single(x => x.SpecializationType == Api.Modeler.SpecializationType));

        public bool IsChild => _element.ParentElement.SpecializationType == SpecializationType;

        public ElementSettingsPersistable ToPersistable()
        {
            return new ElementSettingsPersistable()
            {
                SpecializationType = this.Name,
                DisplayFunction = this.GetSettings().DisplayTextFunction(),
                Icon = GetIcon(this.GetIconFull()) ?? new IconModelPersistable { Type = IconType.FontAwesome, Source = "file-o" },
                ExpandedIcon = GetIcon(this.GetIconFullExpanded()),
                AllowRename = this.GetSettings().AllowRename(),
                AllowAbstract = this.GetSettings().AllowAbstract(),
                AllowGenericTypes = this.GetSettings().AllowGenericTypes(),
                AllowMapping = this.GetSettings().AllowMapping(),
                AllowSorting = this.GetSettings().AllowSorting(),
                AllowFindInView = this.GetSettings().AllowFindInView(),
                AllowTypeReference = !this.GetSettings().TypeReference().IsDisabled(),
                TypeReferenceSetting = !this.GetSettings().TypeReference().IsDisabled() ? new TypeReferenceSettingPersistable()
                {
                    IsRequired = this.GetSettings().TypeReference().IsRequired(),
                    TargetTypes = this.GetSettings().TargetTypes()?.Select(e => e.Name).ToArray(),
                    AllowIsNullable = this.GetSettings().AllowNullable(),
                    AllowIsCollection = this.GetSettings().AllowCollection(),
                } : null,
                DefaultTypeId = this.GetSettings().DefaultTypeId(),
                DiagramSettings = null, // TODO JL / GCB
                ChildElementSettings = this.ChildElementSettings.Select(x => x.ToPersistable()).ToArray(),
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
        public IContextMenu MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ContextMenu.SpecializationType)
            .Select(x => new ContextMenu(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<IElementSettings> ChildElementSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettings.SpecializationType)
            .Select(x => new ElementSettings(x))
            .ToList<IElementSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IDiagramSettings> DiagramSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DiagramSettings.SpecializationType)
            .Select(x => new DiagramSettings(x))
            .ToList<IDiagramSettings>();

        [IntentManaged(Mode.Fully)]
        public IMappingSettings MappingSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingSettings.SpecializationType)
            .Select(x => new MappingSettings(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        protected bool Equals(ElementSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}