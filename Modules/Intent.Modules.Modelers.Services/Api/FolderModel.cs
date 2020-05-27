using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class FolderModel : IHasStereotypes, IMetadataModel, IHasFolder
    {
        public const string SpecializationType = "Folder";
        public FolderModel(IElement element)
        {
            if (element.SpecializationType != "Folder")
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;
            Folder = element.ParentElement != null ? new FolderModel(element.ParentElement) : null;
            IsPackage = false;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        public FolderModel Folder { get; }

        public bool IsPackage { get; }

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

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
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IList<DTOModel> DTOs => _element.ChildElements
            .Where(x => x.SpecializationType == DTOModel.SpecializationType)
            .Select(x => new DTOModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<EnumModel> Enums => _element.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ServiceModel> Services => _element.ChildElements
            .Where(x => x.SpecializationType == ServiceModel.SpecializationType)
            .Select(x => new ServiceModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
    }
}