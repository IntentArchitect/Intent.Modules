using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Api
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
            .GetElementsOfType(ClassModel.SpecializationTypeId)
            .Select(x => new ClassModel(x))
            .ToList();

        public IList<CommentModel> Comments => UnderlyingPackage.ChildElements
            .GetElementsOfType(CommentModel.SpecializationTypeId)
            .Select(x => new CommentModel(x))
            .ToList();

        public IList<DiagramModel> Diagrams => UnderlyingPackage.ChildElements
            .GetElementsOfType(DiagramModel.SpecializationTypeId)
            .Select(x => new DiagramModel(x))
            .ToList();

        public IList<EnumModel> Enums => UnderlyingPackage.ChildElements
            .GetElementsOfType(EnumModel.SpecializationTypeId)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<DataContractModel> DomainObjects => UnderlyingPackage.ChildElements
            .GetElementsOfType(DataContractModel.SpecializationTypeId)
            .Select(x => new DataContractModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();
    }
}