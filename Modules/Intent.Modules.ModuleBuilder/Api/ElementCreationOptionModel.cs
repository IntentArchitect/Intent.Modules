using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class ElementCreationOptionModel : IHasStereotypes, IMetadataModel, ICreationOptionModel, IHasName, IHasTypeReference
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Element Creation Option";

        [IntentManaged(Mode.Ignore)]
        public ElementCreationOptionModel(IElement element, string requiredType = SpecializationType)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

            //Text = element.Name;
            //Shortcut = element.TypeReference.Element.GetStereotypeProperty<string>("Default Creation Options", "Shortcut");
            //DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{element.TypeReference.Element.Name.ToCSharpIdentifier()}";
            Icon = element.TypeReference.Element.GetStereotypeProperty<IIconModel>("Settings", "Icon");
            Type = new ElementSettingsModel((IElement)TypeReference.Element);
            //AllowMultiple = element.GetStereotypeProperty("Creation Options", "Allow Multiple", true);
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public string ApiModelName => Type.ApiModelName;

        public bool AllowMultiple()
        {
            return this.GetOptionSettings().AllowMultiple();
        }

        public ElementCreationOption ToPersistable()
        {
            return new ElementCreationOption
            {
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                Type = ElementType.Element,
                SpecializationTypeId = _element.TypeReference.Element.Id,
                SpecializationType = _element.TypeReference.Element.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{_element.TypeReference.Element.Name.ToCSharpIdentifier()}",
                Icon = Icon?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "file-o" },
                AllowMultiple = this.GetOptionSettings().AllowMultiple()
            };
        }

        public IIconModel Icon { get; }

        public ElementSettingsModel Type { get; }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }


        [IntentManaged(Mode.Fully)]
        public bool Equals(ElementCreationOptionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ElementCreationOptionModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
        public const string SpecializationTypeId = "5fa12f89-da1e-49c5-b8e9-427b22407f19";

        public string Comment => _element.Comment;
    }
}