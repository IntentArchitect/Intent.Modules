using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ComponentViewModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Component View";
        public const string SpecializationTypeId = "624513a6-cba8-4dde-8ebe-6b19f00f0364";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ComponentViewModel(IElement element, string requiredType = SpecializationType)
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

        public IList<DisplayComponentModel> CustomComponents => _element.ChildElements
            .GetElementsOfType(DisplayComponentModel.SpecializationTypeId)
            .Select(x => new DisplayComponentModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ComponentViewModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentViewModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ComponentViewModelExtensions
    {

        public static bool IsComponentViewModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ComponentViewModel.SpecializationTypeId;
        }

        public static ComponentViewModel AsComponentViewModel(this ICanBeReferencedType type)
        {
            return type.IsComponentViewModel() ? new ComponentViewModel((IElement)type) : null;
        }
    }
}