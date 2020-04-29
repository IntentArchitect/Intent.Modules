using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class AssociationCreationOptionModel : IHasStereotypes, IMetadataModel, ICreationOptionModel
    {
        public const string SpecializationType = "Association Creation Option";
        protected readonly IElement _element;

        public AssociationCreationOptionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Type = new AssociationSettingsModel(TypeReference.Element);

        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IIconModel Icon { get; }

        public AssociationSettingsModel Type { get; }

        public bool AllowMultiple()
        {
            return this.GetOptionSettings().AllowMultiple();
        }

        public ElementCreationOption ToPersistable()
        {
            return new ElementCreationOption
            {
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                Type = ElementType.Association,
                SpecializationType = this.Type.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{_element.TypeReference.Element.Name.ToCSharpIdentifier()}",
                Icon = Icon?.ToPersistable() ?? new IconModelPersistable() { Type = (IconType) Metadata.Models.IconType.UrlImagePath, Source = "./img/icons/uml/Association_256x.png" },
                AllowMultiple = this.GetOptionSettings().AllowMultiple()
            };
        }

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationCreationOptionModel other)
        {
            return Equals(_element, other._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationCreationOptionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
    }
}