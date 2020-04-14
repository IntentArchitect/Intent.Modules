using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FolderModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Folder";
        public FolderModel(IElement element)
        {
            if (element.SpecializationType != "Folder")
            {
                throw new Exception($"Cannot create a folder from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            Id = element.Id;
            Name = element.Name;
            ParentFolder = element.ParentElement != null ? new FolderModel(element.ParentElement) : null;
            IsPackage = false;
            Stereotypes = element.Stereotypes;
        }

        public string Id { get; }
        public string Name { get; }
        public FolderModel ParentFolder { get; }
        public bool IsPackage { get; }
        public IEnumerable<IStereotype> Stereotypes { get; }

        protected bool Equals(FolderModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FolderModel)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IList<DTOModel> DTOs => _element.ChildElements
            .Where(x => x.SpecializationType == Api.DTOModel.SpecializationType)
            .Select(x => new DTOModel(x))
            .ToList<DTOModel>();

        [IntentManaged(Mode.Fully)]
        public IList<EnumModel> Enums => _element.ChildElements
            .Where(x => x.SpecializationType == Api.EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList<EnumModel>();

        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .Where(x => x.SpecializationType == Api.FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList<FolderModel>();

        [IntentManaged(Mode.Fully)]
        public IList<ServiceModel> Services => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ServiceModel.SpecializationType)
            .Select(x => new ServiceModel(x))
            .ToList<ServiceModel>();

        [IntentManaged(Mode.Fully)]
        public IList<TypeDefinitionModel> Types => _element.ChildElements
            .Where(x => x.SpecializationType == Api.TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList<TypeDefinitionModel>();
    }
}