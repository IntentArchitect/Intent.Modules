using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using TypeDefinitionModel = Intent.Modelers.Domain.Api.TypeDefinitionModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Domain.Api
{
    [IntentManaged(Mode.Merge)]
    public class DomainPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Domain Package";
        public const string SpecializationTypeId = "1a824508-4623-45d9-accc-f572091ade5a";

        [IntentManaged(Mode.Ignore)]
        public DomainPackageModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<ClassModel> Classes => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ClassModel.SpecializationType)
            .Select(x => new ClassModel(x))
            .ToList();

        public IList<CommentModel> Comments => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == CommentModel.SpecializationType)
            .Select(x => new CommentModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == DiagramModel.SpecializationType)
            .Select(x => new DiagramModel(x))
            .ToList();

        public IList<EnumModel> Enums => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();
    }
}