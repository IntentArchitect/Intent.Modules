using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ContextMenuModel
        : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Context Menu";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ContextMenuModel(IElement element, string requiredType = SpecializationType)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element;
            TypeOrder = ElementCreations.Select(x => new TypeOrder(x))
                .Concat(AssociationCreations.Select(x => new TypeOrder(x)))
                .Distinct().ToList();
            if (StereotypeDefinitionCreation != null)
            {
                TypeOrder.Add(new TypeOrder(StereotypeDefinitionCreation));
            }
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IList<ElementCreationOptionModel> ElementCreations => _element.ChildElements
            .GetElementsOfType(ElementCreationOptionModel.SpecializationTypeId)
            .Select(x => new ElementCreationOptionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<AssociationCreationOptionModel> AssociationCreations => _element.ChildElements
            .GetElementsOfType(AssociationCreationOptionModel.SpecializationTypeId)
            .Select(x => new AssociationCreationOptionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public StereotypeDefinitionCreationOptionModel StereotypeDefinitionCreation => _element.ChildElements
            .GetElementsOfType(StereotypeDefinitionCreationOptionModel.SpecializationTypeId)
            .Select(x => new StereotypeDefinitionCreationOptionModel(x))
            .SingleOrDefault();

        public IList<TypeOrder> TypeOrder { get; }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ContextMenuModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContextMenuModel)obj);
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
        public const string SpecializationTypeId = "d45e383d-90ba-4b64-aca7-34ca100cea21";
    }
}