using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class AssociationEventSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Association Event Settings";
        public const string SpecializationTypeId = "1bdee938-8569-4fbf-84b4-8522d2cbee32";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AssociationEventSettingsModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public IList<AssociationEventHandlerModel> OnLoadedEvents => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        public IList<AssociationEventHandlerModel> OnCreatedEvents => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        public IList<AssociationEventHandlerModel> OnChangedEvents => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        public IList<AssociationEventHandlerModel> OnNameChangedEvents => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        public IList<AssociationEventHandlerModel> OnTypeChangedEvents => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        public IList<AssociationEventHandlerModel> OnDeleteds => _element.ChildElements
            .GetElementsOfType(AssociationEventHandlerModel.SpecializationTypeId)
            .Select(x => new AssociationEventHandlerModel(x))
            .ToList();

        [IntentManaged(Mode.Ignore)]
        public List<MacroPersistable> ToPersistable()
        {
            // TODO: The OnCreatedEvents & OnLoadedEvents returns all ElementMacroModels. Need solution
            return OnCreatedEvents.Select(x => x.ToPersistable()).ToList();
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AssociationEventSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationEventSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationEventSettingsModelExtensions
    {

        public static bool IsAssociationEventSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationEventSettingsModel.SpecializationTypeId;
        }

        public static AssociationEventSettingsModel AsAssociationEventSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationEventSettingsModel() ? new AssociationEventSettingsModel((IElement)type) : null;
        }
    }
}