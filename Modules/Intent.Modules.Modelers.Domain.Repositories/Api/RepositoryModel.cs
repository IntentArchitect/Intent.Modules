using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Repositories.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class RepositoryModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "Repository";
        public const string SpecializationTypeId = "96ffceb2-a70a-4b69-869b-0df436c470c3";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public RepositoryModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference EntityType => TypeReference?.Element != null ? TypeReference : null;

        public IElement InternalElement => _element;

        public IList<OperationModel> Operations => _element.ChildElements
            .GetElementsOfType(OperationModel.SpecializationTypeId)
            .Select(x => new OperationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(RepositoryModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RepositoryModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class RepositoryModelExtensions
    {

        public static bool IsRepositoryModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == RepositoryModel.SpecializationTypeId;
        }

        public static RepositoryModel AsRepositoryModel(this ICanBeReferencedType type)
        {
            return type.IsRepositoryModel() ? new RepositoryModel((IElement)type) : null;
        }
    }
}