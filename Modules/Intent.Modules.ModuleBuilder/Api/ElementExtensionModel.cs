using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ElementExtensionModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Element Extension";
        protected readonly IElement _element;

        public ElementExtensionModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public IList<DiagramSettingsModel> DiagramSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DiagramSettingsModel.SpecializationType)
            .Select(x => new DiagramSettingsModel(x))
            .ToList<DiagramSettingsModel>();

        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public MappingSettingsModel MappingSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.MappingSettingsModel.SpecializationType)
            .Select(x => new MappingSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ContextMenuModel.SpecializationType)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        public ElementSettingExtensionPersistable ToPersistable()
        {
            return new ElementSettingExtensionPersistable()
            {
                SpecializationType = TypeReference.Element.Name,
                CreationOptions = MenuOptions?.CreationOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = MenuOptions?.TypeOrder.Select((t, index) => new TypeOrderPersistable { Type = t.Type, Order = t.Order?.ToString() }).ToList()
            };
        }

        protected bool Equals(ElementExtensionModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}