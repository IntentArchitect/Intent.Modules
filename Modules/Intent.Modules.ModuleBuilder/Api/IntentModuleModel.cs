using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class IntentModuleModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Intent Module";
        public const string SpecializationTypeId = "d2e543c7-174a-45b4-aebe-a13f9ee90214";

        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel(IPackage package)
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

        [IntentManaged(Mode.Ignore)]
        public string ApiNamespace => this.GetModuleSettings().APINamespace() ?? $"{Name}.Api";

        [IntentManaged(Mode.Ignore)]
        public string NuGetPackageId => this.GetModuleSettings().NuGetPackageId();

        [IntentManaged(Mode.Ignore)]
        public string NuGetPackageVersion => this.GetModuleSettings().NuGetPackageVersion();

        [IntentManaged(Mode.Ignore)]
        public string Version => this.GetModuleSettings().Version();

        public IList<FileTemplateModel> FileTemplates => UnderlyingPackage.ChildElements
            .GetElementsOfType(FileTemplateModel.SpecializationTypeId)
            .Select(x => new FileTemplateModel(x))
            .ToList();

        public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<TemplateRegistrationModel> TemplateRegistrations => UnderlyingPackage.ChildElements
            .GetElementsOfType(TemplateRegistrationModel.SpecializationTypeId)
            .Select(x => new TemplateRegistrationModel(x))
            .ToList();

        public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
            .GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
            .Select(x => new TypeDefinitionModel(x))
            .ToList();

        public IList<FactoryExtensionModel> FactoryExtensions => UnderlyingPackage.ChildElements
            .GetElementsOfType(FactoryExtensionModel.SpecializationTypeId)
            .Select(x => new FactoryExtensionModel(x))
            .ToList();

        public IList<TemplateDecoratorModel> TemplateDecorators => UnderlyingPackage.ChildElements
            .GetElementsOfType(TemplateDecoratorModel.SpecializationTypeId)
            .Select(x => new TemplateDecoratorModel(x))
            .ToList();
    }
}