using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class FolderModel : IHasStereotypes, IMetadataModel, IHasFolder
    {
        public const string SpecializationType = "Folder";
        protected readonly IElement _element;

        public FolderModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = element.ParentElement != null ? new FolderModel(element.ParentElement) : null;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        [IntentManaged(Mode.Fully)]
        public IList<ClassModel> Classes => _element.ChildElements
            .Where(x => x.SpecializationType == ClassModel.SpecializationType)
            .Select(x => new ClassModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();
        [IntentManaged(Mode.Fully)]
        public IList<EnumModel> Enums => _element.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();
        [IntentManaged(Mode.Fully)]
        public IList<CommentModel> Comments => _element.ChildElements
            .Where(x => x.SpecializationType == CommentModel.SpecializationType)
            .Select(x => new CommentModel(x))
            .ToList();
        [IntentManaged(Mode.Fully)]
        public IList<DiagramModel> Diagrams => _element.ChildElements
            .Where(x => x.SpecializationType == DiagramModel.SpecializationType)
            .Select(x => new DiagramModel(x))
            .ToList();
        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(FolderModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FolderModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
    }
}