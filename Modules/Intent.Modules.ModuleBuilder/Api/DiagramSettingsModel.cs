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
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DiagramSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Diagram Settings";
        public const string SpecializationTypeId = "67d711dd-3918-4b28-b2a0-4d845884e17a";
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

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public IList<ElementVisualSettingsModel> ElementVisualSettings => _element.ChildElements
            .GetElementsOfType(ElementVisualSettingsModel.SpecializationTypeId)
            .Select(x => new ElementVisualSettingsModel(x))
            .ToList();

        public IList<AssociationVisualSettingsModel> AssociationVisualSettings => _element.ChildElements
            .GetElementsOfType(AssociationVisualSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationVisualSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Ignore)]
        public DiagramSettings ToPersistable()
        {
            return new DiagramSettings()
            {
                CreationOptions = MenuOptions?.ElementCreations.Select(x => x.ToPersistable()).ToList(),
                ScriptOptions = MenuOptions?.RunScriptOptions.Select(x => x.ToPersistable()).ToList(),
                AddNewElementsTo = DiagramAddNewElementsTo.Package,
                ElementVisualSettings = ElementVisualSettings.Select(x => x.ToPersistable()).ToList(),
                AssociationVisualSettings = AssociationVisualSettings.Select(x => x.ToPersistable()).ToList()
            };
        }

        public bool Equals(DiagramSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiagramSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _element.ToString();
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DiagramSettingsModelExtensions
    {

        public static bool IsDiagramSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DiagramSettingsModel.SpecializationTypeId;
        }

        public static DiagramSettingsModel AsDiagramSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsDiagramSettingsModel() ? new DiagramSettingsModel((IElement)type) : null;
        }
    }
}