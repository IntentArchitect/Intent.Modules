using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Metadata.Models;
using Intent.Modules.ApplicationTemplate.Builder.Model;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.ApplicationTemplate.Builder.Api.ApplicationTemplateModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ApplicationTemplateModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Application Template";
        public const string SpecializationTypeId = "cf385bd9-e92f-453d-a3e1-00b9922b57b3";

        [IntentManaged(Mode.Ignore)]
        public ApplicationTemplateModel(IPackage package)
        {
            if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
            Defaults = new ApplicationTemplateDefaultsModel(this);
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<ComponentGroupModel> Groups => UnderlyingPackage.ChildElements
            .GetElementsOfType(ComponentGroupModel.SpecializationTypeId)
            .Select(x => new ComponentGroupModel(x))
            .ToList();

        public MinimumDependencyVersionsModel MinimumDependencyVersions => UnderlyingPackage.ChildElements
            .GetElementsOfType(MinimumDependencyVersionsModel.SpecializationTypeId)
            .Select(x => new MinimumDependencyVersionsModel(x))
            .SingleOrDefault();

        public IList<ApplicationTemplateSettingsConfigurationModel> SettingsConfigurations => UnderlyingPackage.ChildElements
            .GetElementsOfType(ApplicationTemplateSettingsConfigurationModel.SpecializationTypeId)
            .Select(x => new ApplicationTemplateSettingsConfigurationModel(x))
            .ToList();


        [IntentManaged(Mode.Ignore)]
        public TemplateType TemplateType => ConvertToTemplateTypeEnum(this.GetApplicationTemplateSettings().TemplateType().AsEnum());

        [IntentManaged(Mode.Ignore)]
        public string Version => this.GetApplicationTemplateSettings().Version();

        [IntentManaged(Mode.Ignore)]
        public string SupportedClientVersions => this.GetApplicationTemplateSettings().SupportedClientVersions();

        [IntentManaged(Mode.Ignore)]
        public string DisplayName => this.GetApplicationTemplateSettings().DisplayName();

        [IntentManaged(Mode.Ignore)]
        public ImageDetails? CoverImageUrl => this.GetApplicationTemplateSettings().Images().Select(x => new ImageDetails(x)).FirstOrDefault();

        [IntentManaged(Mode.Ignore)]
        public List<ImageDetails> AdditionalImages => this.GetApplicationTemplateSettings().Images().Skip(1).Select(x => new ImageDetails(x)).ToList();

        [IntentManaged(Mode.Ignore)]
        public string Description => this.GetApplicationTemplateSettings().Description();

        [IntentManaged(Mode.Ignore)]
        public string Authors => this.GetApplicationTemplateSettings().Authors();

        [IntentManaged(Mode.Ignore)]
        public string Priority => this.GetApplicationTemplateSettings().Priority();

        [IntentManaged(Mode.Ignore)]
        public IIconModel Icon => this.GetApplicationTemplateSettings().Icon();

        [IntentManaged(Mode.Ignore)]
        public ApplicationTemplateDefaultsModel Defaults { get; }

        [IntentManaged(Mode.Ignore)]
        private TemplateType ConvertToTemplateTypeEnum(ApplicationTemplateSettings.TemplateTypeOptionsEnum value)
        {
            switch (value)
            {
                case ApplicationTemplateSettings.TemplateTypeOptionsEnum.ArchitectureTemplate:
                    return TemplateType.ApplicationTemplate;
                case ApplicationTemplateSettings.TemplateTypeOptionsEnum.ModuleBuilding:
                    return TemplateType.ModuleBuilding;
                default:
                    return TemplateType.ApplicationTemplate;
            }
        }

        [IntentManaged(Mode.Ignore)]
        public class ApplicationTemplateDefaultsModel
        {
            private readonly ApplicationTemplateModel _model;

            public ApplicationTemplateDefaultsModel(ApplicationTemplateModel model)
            {
                _model = model;
            }

            public string Name => _model.GetApplicationTemplateDefaults()?.Name() ?? "NewApplication";

            public string RelativeOutputLocation => _model.GetApplicationTemplateDefaults()?.RelativeOutputLocation() ?? string.Empty;

            public bool PlaceSolutionAndApplicationInTheSameDirectory => _model.GetApplicationTemplateDefaults()?.PlaceSolutionAndApplicationInTheSameDirectory() ?? false;

            public bool StoreIntentArchitectFilesSeparateToCodebase => _model.GetApplicationTemplateDefaults()?.StoreIntentArchitectFilesSeparateToCodebase() ?? true;

            public bool SetGitIgnoreEntries => _model.GetApplicationTemplateDefaults()?.SetGitignoreEntries() ?? true;

            public bool CreateFolderForSolution => _model.GetApplicationTemplateDefaults()?.CreateFolderForSolution() ?? true;
        }
    }
}