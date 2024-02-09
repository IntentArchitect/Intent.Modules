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
    public class ComponentModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Component";
        public const string SpecializationTypeId = "b1c481e1-e91e-4c29-9817-00ab9cad4b6b";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ComponentModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ComponentInputModel> Inputs => _element.ChildElements
            .GetElementsOfType(ComponentInputModel.SpecializationTypeId)
            .Select(x => new ComponentInputModel(x))
            .ToList();

        public IList<ComponentPropertyModel> Properties => _element.ChildElements
            .GetElementsOfType(ComponentPropertyModel.SpecializationTypeId)
            .Select(x => new ComponentPropertyModel(x))
            .ToList();

        public IList<ComponentCommandModel> Commands => _element.ChildElements
            .GetElementsOfType(ComponentCommandModel.SpecializationTypeId)
            .Select(x => new ComponentCommandModel(x))
            .ToList();

        public ComponentViewModel View => _element.ChildElements
            .GetElementsOfType(ComponentViewModel.SpecializationTypeId)
            .Select(x => new ComponentViewModel(x))
            .SingleOrDefault();

        public IList<ModelDefinitionModel> ModelDefinitions => _element.ChildElements
            .GetElementsOfType(ModelDefinitionModel.SpecializationTypeId)
            .Select(x => new ModelDefinitionModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ComponentModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ComponentModelExtensions
    {

        public static bool IsComponentModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ComponentModel.SpecializationTypeId;
        }

        public static ComponentModel AsComponentModel(this ICanBeReferencedType type)
        {
            return type.IsComponentModel() ? new ComponentModel((IElement)type) : null;
        }
    }
}