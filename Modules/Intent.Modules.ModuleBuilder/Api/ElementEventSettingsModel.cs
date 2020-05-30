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
    public class ElementEventSettingsModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Element Event Settings";
        protected readonly IElement _element;

        public ElementEventSettingsModel(IElement element, string requiredType = SpecializationType)
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

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementEventSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementEventSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<ElementEventHandlerModel> OnCreatedEvents => _element.ChildElements
            .Where(x => x.SpecializationType == ElementEventHandlerModel.SpecializationType)
            .Select(x => new ElementEventHandlerModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ElementEventHandlerModel> OnLoadedEvents => _element.ChildElements
            .Where(x => x.SpecializationType == ElementEventHandlerModel.SpecializationType)
            .Select(x => new ElementEventHandlerModel(x))
            .ToList();

        public List<ElementMacroPersistable> ToPersistable()
        {
            // TODO: The OnCreatedEvents & OnLoadedEvents returns all ElementMacroModels. Need solution
            return OnCreatedEvents.Select(x => x.ToPersistable()).ToList();
        }
    }
}