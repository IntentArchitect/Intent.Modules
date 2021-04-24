using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PackageSettingsModel
        : IMetadataModel, IHasStereotypes, IHasName
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Package Settings";

        [IntentManaged(Mode.Ignore)]
        public PackageSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;
        }

        public static PackageSettingsModel Create(IElement element)
        {
            return element != null ? new PackageSettingsModel(element) : null;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Ignore)]
        public string ApiModelName => $"{Name.ToCSharpIdentifier()}Model";

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .GetElementsOfType(ContextMenuModel.SpecializationTypeId)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();


        public PackageSettingsPersistable ToPersistable()
        {
            return new PackageSettingsPersistable
            {
                SpecializationTypeId = Id,
                SpecializationType = Name,
                DefaultName = this.GetPackageSettings().DefaultName(),
                SortChildren = DetermineSortingOption(),
                Icon = this.GetPackageSettings().Icon()?.ToPersistable(),
                CreationOptions = MenuOptions?.ElementCreations.Select(x => x.ToPersistable())
                    .Concat(MenuOptions.AssociationCreations.Select(x => x.ToPersistable()))
                    .Concat(MenuOptions.StereotypeDefinitionCreation != null ? new[] { MenuOptions.StereotypeDefinitionCreation.ToPersistable() } : new ElementCreationOption[0])
                    .ToList(),
                TypeOrder = MenuOptions?.TypeOrder.Select(x => new TypeOrderPersistable() { Type = x.Type, Order = x.Order?.ToString() }).ToList(),
                RequiredPackages = new string[0],
                Macros = this.EventSettings?.OnCreatedEvents.Select(x => x.ToPersistable()).ToList()
            };
        }

        private SortChildrenOptions DetermineSortingOption()
        {
            if (this.GetPackageSettings().Sorting().IsManually())
            {
                return SortChildrenOptions.Manually;
            }
            if (this.GetPackageSettings().Sorting().IsByTypeThenManually())
            {
                return SortChildrenOptions.SortByTypeThenManually;
            }
            if (this.GetPackageSettings().Sorting().IsByTypeThenByName())
            {
                return SortChildrenOptions.SortByTypeAndName;
            }
            if (this.GetPackageSettings().Sorting().IsByName())
            {
                return SortChildrenOptions.SortByName;
            }
            return SortChildrenOptions.Manually;
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(PackageSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackageSettingsModel)obj);
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

        [IntentManaged(Mode.Fully)]
        public PackageEventSettingsModel EventSettings => _element.ChildElements
            .GetElementsOfType(PackageEventSettingsModel.SpecializationTypeId)
            .Select(x => new PackageEventSettingsModel(x))
            .SingleOrDefault();

        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);

        public const string SpecializationTypeId = "89333f72-3960-4159-bf61-9c40d4b65088";

        public string Comment => _element.Comment;


    }
}