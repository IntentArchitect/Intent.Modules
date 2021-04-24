using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class StereotypeDefinitionCreationOptionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Stereotype-Definition Creation Option";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public StereotypeDefinitionCreationOptionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Icon = element.TypeReference.Element.GetStereotypeProperty<IIconModel>("Settings", "Icon");
            Type = new CoreTypeModel((IElement)TypeReference.Element);
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        public IIconModel Icon { get; }

        public CoreTypeModel Type { get; }

        public ElementCreationOption ToPersistable()
        {
            return new ElementCreationOption
            {
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                Type = ElementType.StereotypeDefinition,
                SpecializationTypeId = this.Type.Name,
                SpecializationType = this.Type.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{_element.TypeReference.Element.Name.ToCSharpIdentifier()}",
                Icon = Icon?.ToPersistable(),
                AllowMultiple = this.GetOptionSettings().AllowMultiple()
            };
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(StereotypeDefinitionCreationOptionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StereotypeDefinitionCreationOptionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
        public const string SpecializationTypeId = "0c721d5d-7721-414c-adaf-4c9f5c5531a9";

        public string Comment => _element.Comment;
    }
}