using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DiagramSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Diagram Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public DiagramSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'DiagramSettingsModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
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
        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public DiagramSettings ToPersistable()
        {
            return new DiagramSettings()
            {
                CreationOptions = MenuOptions?.ElementCreations.Select(x => x.ToPersistable()).ToArray(),
                AddNewElementsTo = DiagramAddNewElementsTo.Package,
                ClassVisualSettings = new ElementVisualSettingsPersistable[0],
                AssociationVisualSettings = new AssociationVisualSettingsPersistable[0]
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(DiagramSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramSettingsModel)obj);
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
        public const string SpecializationTypeId = "67d711dd-3918-4b28-b2a0-4d845884e17a";

        public string Comment => _element.Comment;

        public ElementEventSettingsModel EventSettings => _element.ChildElements
                    .GetElementsOfType(ElementEventSettingsModel.SpecializationTypeId)
                    .Select(x => new ElementEventSettingsModel(x))
                    .SingleOrDefault();
    }
}