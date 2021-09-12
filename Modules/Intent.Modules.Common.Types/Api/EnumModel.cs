using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Common.Types.Api
{
    [IntentManaged(Mode.Merge)]
    public class EnumModel : IHasStereotypes, IMetadataModel, IHasName
    {
        public const string SpecializationType = "Enum";
        public const string SpecializationTypeId = "85fba0e9-9161-4c85-a603-a229ef312beb";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public EnumModel(IElement element, string requiredType = SpecializationType)
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

        public IElement InternalElement => _element;

        public IList<EnumLiteralModel> Literals => _element.ChildElements
            .GetElementsOfType(EnumLiteralModel.SpecializationTypeId)
            .Select(x => new EnumLiteralModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(EnumModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnumModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class EnumModelExtensions
    {

        public static bool IsEnumModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == EnumModel.SpecializationTypeId;
        }

        public static EnumModel ToEnumModel(this ICanBeReferencedType type)
        {
            return type.IsEnumModel() ? new EnumModel((IElement)type) : null;
        }
    }
}