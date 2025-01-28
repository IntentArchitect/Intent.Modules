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
    public class AutoCompleteModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "AutoComplete";
        public const string SpecializationTypeId = "ff1ddb80-16e4-4e63-ace3-4ef96a03b4b0";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AutoCompleteModel(IElement element, string requiredType = SpecializationType)
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

        public string Value => _element.Value;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AutoCompleteModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AutoCompleteModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AutoCompleteModelExtensions
    {

        public static bool IsAutoCompleteModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AutoCompleteModel.SpecializationTypeId;
        }

        public static AutoCompleteModel AsAutoCompleteModel(this ICanBeReferencedType type)
        {
            return type.IsAutoCompleteModel() ? new AutoCompleteModel((IElement)type) : null;
        }
    }
}