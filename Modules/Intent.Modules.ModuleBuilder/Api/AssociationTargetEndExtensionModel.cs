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
    public class AssociationTargetEndExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Association Target End Extension";
        public const string SpecializationTypeId = "1cc6c731-af5f-41a0-9e82-44486f45b903";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AssociationTargetEndExtensionModel(IElement element, string requiredType = SpecializationTypeId)
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

        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AssociationTargetEndExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationTargetEndExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public AssociationEndSettingExtensionPersistable ToPersistable()
        {
            return new AssociationEndSettingExtensionPersistable
            {
                TypeReferenceExtension = new TypeReferenceExtensionSettingPersistable()
                {
                    IsRequired = true,
                    TargetTypes = (this.GetAssociationEndExtensionSettings().TargetTypes()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .Concat(this.GetAssociationEndExtensionSettings().TargetTraits()?.Select(x => new TargetTypePersistable() { Type = x.Name, TypeId = x.Id }) ?? new List<TargetTypePersistable>())
                        .ToArray(),
                    DefaultTypeId = string.IsNullOrWhiteSpace(this.GetAssociationEndExtensionSettings().DefaultTypeId()) ? this.GetAssociationEndExtensionSettings().DefaultTypeId() : null,
                    AllowIsNavigable = Enum.TryParse<BooleanExtensionOptions>(this.GetAssociationEndExtensionSettings().AllowNullable().Value, out var allowTargetIsNavigable) ? allowTargetIsNavigable : BooleanExtensionOptions.Inherit,
                    AllowIsNullable = Enum.TryParse<BooleanExtensionOptions>(this.GetAssociationEndExtensionSettings().AllowNullable().Value, out var allowTargetIsNullable) ? allowTargetIsNullable : BooleanExtensionOptions.Inherit,
                    AllowIsCollection = Enum.TryParse<BooleanExtensionOptions>(this.GetAssociationEndExtensionSettings().AllowCollection().Value, out var allowTargetIsCollection) ? allowTargetIsCollection : BooleanExtensionOptions.Inherit,
                    DisplayName = this.GetAssociationEndExtensionSettings().DisplayName(),
                    Hint = this.GetAssociationEndExtensionSettings().Hint()
                },
                ContextMenuOptions = MenuOptions?.ToPersistable(),
                //CreationOptions = this.MenuOptions?.ToCreationOptionsPersistable(),
                //ScriptOptions = MenuOptions?.RunScriptOptions.Select(x => x.ToPersistable()).ToList(),
                //MappingOptions = MenuOptions?.MappingOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = this.MenuOptions?.TypeOrder.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationTargetEndExtensionModelExtensions
    {

        public static bool IsAssociationTargetEndExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationTargetEndExtensionModel.SpecializationTypeId;
        }

        public static AssociationTargetEndExtensionModel AsAssociationTargetEndExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationTargetEndExtensionModel() ? new AssociationTargetEndExtensionModel((IElement)type) : null;
        }
    }
}