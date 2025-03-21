using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PackageExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Package Extension";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Ignore)]
        public string ApiModelName => $"{Name.ToCSharpIdentifier()}Model";

        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .GetElementsOfType(ElementExtensionModel.SpecializationTypeId)
            .Select(x => new ElementExtensionModel(x))
            .ToList();

        public PackageSettingsExtensionPersistable ToPersistable()
        {
            return new PackageSettingsExtensionPersistable
            {
                SpecializationTypeId = TypeReference.Element.Id,
                SpecializationType = TypeReference.Element.Name,
                ContextMenuOptions = MenuOptions?.ToPersistable(),
                CreationOptions = MenuOptions?.ElementCreations.Select(x => x.ToPersistableOld())
                    .Concat(MenuOptions.AssociationCreations.Select(x => x.ToPersistableOld()))
                    .Concat(MenuOptions.StereotypeDefinitionCreation != null ? new[] { MenuOptions.StereotypeDefinitionCreation.ToPersistableOld() } : new ElementCreationOptionOld[0])
                    .ToList(),
                ScriptOptions = MenuOptions?.RunScriptOptions.Select(x => x.ToPersistable()).ToList(),
                TypeOrder = MenuOptions?.TypeOrder.Select(x => x.ToPersistable()).ToList(),
                RequiredPackages = new string[0],
                Macros = this.EventSettings?.ToPersistable(),
                ChildElementExtensions = this.ElementExtensions.Select(x => x.ToPersistable()).ToArray(),
            };
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(PackageExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageExtensionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "ab4152df-3add-4a08-81b6-0fefc7cbb204";

        public string Comment => _element.Comment;

        public PackageEventSettingsModel EventSettings => _element.ChildElements
            .GetElementsOfType(PackageEventSettingsModel.SpecializationTypeId)
            .Select(x => new PackageEventSettingsModel(x))
            .SingleOrDefault();
    }

    [IntentManaged(Mode.Fully)]
    public static class PackageExtensionModelExtensions
    {

        public static bool IsPackageExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == PackageExtensionModel.SpecializationTypeId;
        }

        public static PackageExtensionModel AsPackageExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsPackageExtensionModel() ? new PackageExtensionModel((IElement)type) : null;
        }
    }
}