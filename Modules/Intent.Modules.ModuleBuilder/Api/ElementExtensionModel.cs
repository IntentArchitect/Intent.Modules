using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementExtensionModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Element Extension";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ElementExtensionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public IList<DiagramSettingsModel> DiagramSettings => _element.ChildElements
            .Where(x => x.SpecializationType == DiagramSettingsModel.SpecializationType)
            .Select(x => new DiagramSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public MappingSettingsModel MappingSettings => _element.ChildElements
            .Where(x => x.SpecializationType == MappingSettingsModel.SpecializationType)
            .Select(x => new MappingSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == ContextMenuModel.SpecializationType)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        public ElementSettingExtensionPersistable ToPersistable()
        {
            return new ElementSettingExtensionPersistable()
            {
                SpecializationType = TypeReference.Element.Name,
                SpecializationTypeId = TypeReference.Element.Id,
                CreationOptions = this.MenuOptions?.ElementCreations.Select(x => x.ToPersistable())
                    .Concat(this.MenuOptions.AssociationCreations.Select(x => x.ToPersistable()))
                    .Concat(MenuOptions.StereotypeDefinitionCreation != null ? new[] { MenuOptions.StereotypeDefinitionCreation.ToPersistable() } : new ElementCreationOption[0])
                    .ToList(),
                TypeOrder = MenuOptions?.TypeOrder.Select((t, index) => new TypeOrderPersistable { Type = t.Type, Order = t.Order?.ToString() }).ToList(),
                MappingSettings = MappingSettings?.ToPersistable()
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementExtensionModel)obj);
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
        public const string SpecializationTypeId = "e3c7b1ca-f080-45c1-b56f-8d44226c8e20";
    }
}