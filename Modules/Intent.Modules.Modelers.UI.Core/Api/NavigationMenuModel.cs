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
    public class NavigationMenuModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Navigation Menu";
        public const string SpecializationTypeId = "d7282bf2-1626-4b8b-9446-1d530527db06";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public NavigationMenuModel(IElement element, string requiredType = SpecializationType)
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

        public NavigationBrandLogoModel BrandLogo => _element.ChildElements
            .GetElementsOfType(NavigationBrandLogoModel.SpecializationTypeId)
            .Select(x => new NavigationBrandLogoModel(x))
            .SingleOrDefault();

        public IList<NavigationItemModel> NavigationItems => _element.ChildElements
            .GetElementsOfType(NavigationItemModel.SpecializationTypeId)
            .Select(x => new NavigationItemModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(NavigationMenuModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NavigationMenuModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class NavigationMenuModelExtensions
    {

        public static bool IsNavigationMenuModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == NavigationMenuModel.SpecializationTypeId;
        }

        public static NavigationMenuModel AsNavigationMenuModel(this ICanBeReferencedType type)
        {
            return type.IsNavigationMenuModel() ? new NavigationMenuModel((IElement)type) : null;
        }
    }
}