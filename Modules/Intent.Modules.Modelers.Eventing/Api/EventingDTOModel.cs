using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Eventing.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class EventingDTOModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "Eventing DTO";
        public const string SpecializationTypeId = "544f1d57-27ce-4985-a4ec-cc01568d72b0";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public EventingDTOModel(IElement element, string requiredType = SpecializationType)
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

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public IElement InternalElement => _element;

        public IList<EventingDTOFieldModel> Fields => _element.ChildElements
            .GetElementsOfType(EventingDTOFieldModel.SpecializationTypeId)
            .Select(x => new EventingDTOFieldModel(x))
            .ToList();

        [IntentManaged(Mode.Ignore)]
        public bool IsAbstract => _element.IsAbstract;

        [IntentManaged(Mode.Ignore)]
        public EventingDTOModel ParentDto => this.Generalizations().Select(x => new EventingDTOModel((IElement)x.Element)).SingleOrDefault();

        [IntentManaged(Mode.Ignore)]
        public ITypeReference ParentDtoTypeReference => this.Generalizations().SingleOrDefault()?.TypeReference;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(EventingDTOModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventingDTOModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class EventingDTOModelExtensions
    {

        public static bool IsEventingDTOModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == EventingDTOModel.SpecializationTypeId;
        }

        public static EventingDTOModel AsEventingDTOModel(this ICanBeReferencedType type)
        {
            return type.IsEventingDTOModel() ? new EventingDTOModel((IElement)type) : null;
        }
    }
}