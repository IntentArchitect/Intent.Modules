using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ContextMenuModel
        : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
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
            TypeOrder = _element.ChildElements.Select(x =>
                {
                    if (x.IsElementCreationOptionModel()) return new TypeOrder(x.AsElementCreationOptionModel());
                    if (x.IsAssociationCreationOptionModel()) return new TypeOrder(x.AsAssociationCreationOptionModel());
                    return null;
                })
                .Where(x => x != null)
                .Distinct()
                .ToList();

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

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        public IList<ElementCreationOptionModel> ElementCreations => _element.ChildElements
            .GetElementsOfType(ElementCreationOptionModel.SpecializationTypeId)
            .Select(x => new ElementCreationOptionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        public IList<AssociationCreationOptionModel> AssociationCreations => _element.ChildElements
            .GetElementsOfType(AssociationCreationOptionModel.SpecializationTypeId)
            .Select(x => new AssociationCreationOptionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        public StereotypeDefinitionCreationOptionModel StereotypeDefinitionCreation => _element.ChildElements
            .GetElementsOfType(StereotypeDefinitionCreationOptionModel.SpecializationTypeId)
            .Select(x => new StereotypeDefinitionCreationOptionModel(x))
            .SingleOrDefault();

        public IList<TypeOrder> TypeOrder { get; }

        [Obsolete("Replaced with ToPersistable() which returns List<ContextMenuOption>")]
        public List<ElementCreationOptionOld> ToCreationOptionsPersistable()
        {
            return _element.ChildElements.Select(x =>
                {
                    if (x.IsElementCreationOptionModel()) return x.AsElementCreationOptionModel().ToPersistableOld();
                    if (x.IsAssociationCreationOptionModel()) return x.AsAssociationCreationOptionModel().ToPersistableOld();
                    if (x.IsStereotypeDefinitionCreationOptionModel()) return x.AsStereotypeDefinitionCreationOptionModel().ToPersistableOld();
                    return null;
                })
                .Where(x => x != null)

                .ToList();
        }

        public List<ContextMenuOption> ToPersistable()
        {
            return _element.ChildElements.Select(x =>
                {
                    if (x.IsElementCreationOptionModel()) return x.AsElementCreationOptionModel().ToPersistable();
                    if (x.IsAssociationCreationOptionModel()) return x.AsAssociationCreationOptionModel().ToPersistable();
                    if (x.IsStereotypeDefinitionCreationOptionModel()) return x.AsStereotypeDefinitionCreationOptionModel().ToPersistable();
                    if (x.IsRunScriptOptionModel()) return x.AsRunScriptOptionModel().ToPersistable();
                    if (x.IsMappingOptionModel()) return x.AsMappingOptionModel().ToPersistable();
                    return null;
                })
                .Where(x => x != null)
                .ToList();
        }

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

        public string Comment => _element.Comment;

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        [Obsolete("Replaced with ToPersistable() which returns List<ContextMenuOption>")]
        public IList<RunScriptOptionModel> RunScriptOptions => _element.ChildElements
            .GetElementsOfType(RunScriptOptionModel.SpecializationTypeId)
            .Select(x => new RunScriptOptionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        [Obsolete("Replaced with ToPersistable() which returns List<ContextMenuOption>")]
        public IList<MappingOptionModel> MappingOptions => _element.ChildElements
            .GetElementsOfType(MappingOptionModel.SpecializationTypeId)
            .Select(x => new MappingOptionModel(x))
            .ToList();
    }

    [IntentManaged(Mode.Fully)]
    public static class ContextMenuModelExtensions
    {

        public static bool IsContextMenuModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ContextMenuModel.SpecializationTypeId;
        }

        public static ContextMenuModel AsContextMenuModel(this ICanBeReferencedType type)
        {
            return type.IsContextMenuModel() ? new ContextMenuModel((IElement)type) : null;
        }
    }
}