using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class LayoutModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Layout";
        public const string SpecializationTypeId = "776a9393-6b23-4a8c-8937-fd7e833fa0ef";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public LayoutModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public LayoutHeaderModel Header => _element.ChildElements
            .GetElementsOfType(LayoutHeaderModel.SpecializationTypeId)
            .Select(x => new LayoutHeaderModel(x))
            .SingleOrDefault();

        public LayoutSiderModel Sider => _element.ChildElements
            .GetElementsOfType(LayoutSiderModel.SpecializationTypeId)
            .Select(x => new LayoutSiderModel(x))
            .SingleOrDefault();

        public LayoutBodyModel Body => _element.ChildElements
            .GetElementsOfType(LayoutBodyModel.SpecializationTypeId)
            .Select(x => new LayoutBodyModel(x))
            .SingleOrDefault();

        public LayoutFooterModel Footer => _element.ChildElements
            .GetElementsOfType(LayoutFooterModel.SpecializationTypeId)
            .Select(x => new LayoutFooterModel(x))
            .SingleOrDefault();

        public IList<PropertyModel> Properties => _element.ChildElements
            .GetElementsOfType(PropertyModel.SpecializationTypeId)
            .Select(x => new PropertyModel(x))
            .ToList();

        public IList<ComponentOperationModel> Operations => _element.ChildElements
            .GetElementsOfType(ComponentOperationModel.SpecializationTypeId)
            .Select(x => new ComponentOperationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(LayoutModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LayoutModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class LayoutModelExtensions
    {

        public static bool IsLayoutModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == LayoutModel.SpecializationTypeId;
        }

        public static LayoutModel AsLayoutModel(this ICanBeReferencedType type)
        {
            return type.IsLayoutModel() ? new LayoutModel((IElement)type) : null;
        }
    }
}