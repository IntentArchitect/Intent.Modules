using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PackageSettingsModel
        : IHasStereotypes, IMetadataModel
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Package Settings";

        public PackageSettingsModel(IElement element)
        {
            if (element?.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
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

        [IntentManaged(Mode.Fully)]
        public ContextMenuModel MenuOptions => _element.ChildElements
            .Where(x => x.SpecializationType == ContextMenuModel.SpecializationType)
            .Select(x => new ContextMenuModel(x))
            .SingleOrDefault();

        public PackageSettingsPersistable ToPersistable()
        {
            return new PackageSettingsPersistable
            {
                CreationOptions = this?.MenuOptions.ElementCreations.Select(x => x.ToPersistable())
                    .Concat(this.MenuOptions.AssociationCreations.Select(x => x.ToPersistable()))
                    .Concat(MenuOptions.StereotypeDefinitionCreation != null ? new[] { MenuOptions.StereotypeDefinitionCreation.ToPersistable() } : new ElementCreationOption[0])
                    .ToList(),
                TypeOrder = this?.MenuOptions.TypeOrder.Select(x => new TypeOrderPersistable() { Type = x.Type, Order = x.Order?.ToString() }).ToList()
            };
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


    }
}