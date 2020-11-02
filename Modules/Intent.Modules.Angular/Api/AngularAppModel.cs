using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge)]
    public class AngularAppModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Angular App";
        public const string SpecializationTypeId = "fc44141e-8079-4f63-894b-f1a26736c991";

        [IntentManaged(Mode.Ignore)]
        public AngularAppModel(IPackage package)
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

        public IList<EnumModel> Enums => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == EnumModel.SpecializationType)
            .Select(x => new EnumModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == FolderModel.SpecializationType)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<ModelDefinitionModel> ModelDefinitions => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ModelDefinitionModel.SpecializationType)
            .Select(x => new ModelDefinitionModel(x))
            .ToList();

        public IList<ModuleModel> Modules => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == ModuleModel.SpecializationType)
            .Select(x => new ModuleModel(x))
            .ToList();

        public IList<AngularServiceModel> Services => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == AngularServiceModel.SpecializationType)
            .Select(x => new AngularServiceModel(x))
            .ToList();

        public IList<TypeDefinitionModel> TypeDefinitions => UnderlyingPackage.ChildElements
            .Where(x => x.SpecializationType == TypeDefinitionModel.SpecializationType)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();
    }
}