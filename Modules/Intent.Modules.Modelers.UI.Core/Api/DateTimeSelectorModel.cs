using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DateTimeSelectorModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Date Time Selector";
        public const string SpecializationTypeId = "9451fcdc-9406-4323-b354-2eaaabbf9ac5";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DateTimeSelectorModel(IElement element, string requiredType = SpecializationType)
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

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DateTimeSelectorModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DateTimeSelectorModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DateTimeSelectorModelExtensions
    {

        public static bool IsDateTimeSelectorModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DateTimeSelectorModel.SpecializationTypeId;
        }

        public static DateTimeSelectorModel AsDateTimeSelectorModel(this ICanBeReferencedType type)
        {
            return type.IsDateTimeSelectorModel() ? new DateTimeSelectorModel((IElement)type) : null;
        }
    }
}