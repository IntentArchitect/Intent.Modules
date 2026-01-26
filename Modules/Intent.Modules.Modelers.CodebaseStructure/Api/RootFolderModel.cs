using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    [IntentManaged(Mode.Fully)]
    public class RootFolderModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Root Folder";
        public const string SpecializationTypeId = "07e7b690-a59d-4b72-8440-4308a121d32c";

        [IntentManaged(Mode.Ignore)]
        public RootFolderModel(IPackage package)
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

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<OutputAnchorModel> OutputAnchors => UnderlyingPackage.ChildElements
            .GetElementsOfType(OutputAnchorModel.SpecializationTypeId)
            .Select(x => new OutputAnchorModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => UnderlyingPackage.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

    }
}