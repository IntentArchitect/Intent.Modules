using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.OutputTargets.Folders.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    [IntentManaged(Mode.Merge)]
    public class RootLocationPackageModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Root Location Package";
        public const string SpecializationTypeId = "fbca4a65-b376-482c-a23d-32da9908cac9";

        [IntentManaged(Mode.Ignore)]
        public RootLocationPackageModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        [IntentManaged(Mode.Ignore)]
        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new RootFolderOutputTarget(this);
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

        public IList<RoleModel> Roles => UnderlyingPackage.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => UnderlyingPackage.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();
    }
}