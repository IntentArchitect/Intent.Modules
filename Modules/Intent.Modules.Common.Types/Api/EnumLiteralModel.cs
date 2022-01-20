using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Common.Types.Api
{
    [IntentManaged(Mode.Merge)]
    public class EnumLiteralModel : IHasStereotypes, IMetadataModel, IHasName
    {
        public const string SpecializationType = "Enum-Literal";
        public const string SpecializationTypeId = "4215f417-25d2-4509-9309-5076a1452eaa";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public EnumLiteralModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public string Value => _element.Value;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(EnumLiteralModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnumLiteralModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class EnumLiteralModelExtensions
    {

        public static bool IsEnumLiteralModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == EnumLiteralModel.SpecializationTypeId;
        }

        public static EnumLiteralModel AsEnumLiteralModel(this ICanBeReferencedType type)
        {
            return type.IsEnumLiteralModel() ? new EnumLiteralModel((IElement)type) : null;
        }
    }
}