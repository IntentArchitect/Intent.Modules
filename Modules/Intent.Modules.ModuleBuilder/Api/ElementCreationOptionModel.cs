using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class ElementCreationOptionModel : IHasStereotypes, IMetadataModel, ICreationOptionModel, IHasName, IHasTypeReference, IElementWrapper
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

        [Obsolete]
        public ElementCreationOptionOld ToPersistableOld()
        {
            return new ElementCreationOptionOld
            {
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                Type = ContextMenuOptionType.Element,
                SpecializationTypeId = _element.TypeReference.Element.Id,
                SpecializationType = _element.TypeReference.Element.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{_element.TypeReference.Element.Name.ToCSharpIdentifier()}",
                Icon = Icon?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "file-o" },
                AllowMultiple = this.GetOptionSettings().AllowMultiple(),
                IsOptionVisibleFunction = this.GetOptionSettings().IsOptionVisibleFunction(),
                HasTopDivider = this.GetOptionSettings().TopDivider(),
                HasBottomDivider = this.GetOptionSettings().BottomDivider(),
            };
        }

        public ContextMenuOption ToPersistable()
        {
            return new ElementCreationOption
            {
                Type = ContextMenuOptionType.Element,
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                SpecializationTypeId = _element.TypeReference.Element.Id,
                SpecializationType = _element.TypeReference.Element.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                DefaultName = this.GetOptionSettings().DefaultName() ?? $"New{_element.TypeReference.Element.Name.ToCSharpIdentifier()}",
                Icon = Icon?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "file-o" },
                AllowMultiple = this.GetOptionSettings().AllowMultiple(),
                IsOptionVisibleFunction = this.GetOptionSettings().IsOptionVisibleFunction(),
                HasTopDivider = this.GetOptionSettings().TopDivider(),
                HasBottomDivider = this.GetOptionSettings().BottomDivider(),
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

    [IntentManaged(Mode.Fully)]
    public static class ElementCreationOptionModelExtensions
    {

        public static bool IsElementCreationOptionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ElementCreationOptionModel.SpecializationTypeId;
        }

        public static ElementCreationOptionModel AsElementCreationOptionModel(this ICanBeReferencedType type)
        {
            return type.IsElementCreationOptionModel() ? new ElementCreationOptionModel((IElement)type) : null;
        }
    }
}