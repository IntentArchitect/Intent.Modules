using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappingOptionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Mapping Option";
        public const string SpecializationTypeId = "00a1eb98-1fc4-4421-9d1c-8733f6bc2111";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappingOptionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;


        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappingOptionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingOptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Ignore)]
        public MappingOption ToPersistable()
        {
            return new MappingOption()
            {
                Order = this.GetOptionSettings().TypeOrder()?.ToString(),
                MappingTypeId = _element.TypeReference.Element.Id,
                MappingType = _element.TypeReference.Element.Name,
                Text = this.Name,
                Shortcut = this.GetOptionSettings().Shortcut(),
                MacShortcut = this.GetOptionSettings().ShortcutMacOS(),
                Icon = this.GetOptionSettings().Icon()?.ToPersistable() ?? new IconModelPersistable() { Type = IconType.FontAwesome, Source = "arrow-circle-right" },
                IsOptionVisibleFunction = this.GetOptionSettings().IsOptionVisibleFunction(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappingOptionModelExtensions
    {

        public static bool IsMappingOptionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappingOptionModel.SpecializationTypeId;
        }

        public static MappingOptionModel AsMappingOptionModel(this ICanBeReferencedType type)
        {
            return type.IsMappingOptionModel() ? new MappingOptionModel((IElement)type) : null;
        }
    }
}