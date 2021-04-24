using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    [IntentManaged(Mode.Merge)]
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

        [IntentManaged(Mode.Ignore)]
        public string Version => this.GetApplicationTemplateSettings().Version();

        [IntentManaged(Mode.Ignore)]
        public string SupportedClientVersions => this.GetApplicationTemplateSettings().SupportedClientVersions();

        [IntentManaged(Mode.Ignore)]
        public string DisplayName => this.GetApplicationTemplateSettings().DisplayName();

        [IntentManaged(Mode.Ignore)]
        public string Description => this.GetApplicationTemplateSettings().Description();

        [IntentManaged(Mode.Ignore)]
        public string Authors => this.GetApplicationTemplateSettings().Authors();

        [IntentManaged(Mode.Ignore)]
        public string Priority => this.GetApplicationTemplateSettings().Priority();

        [IntentManaged(Mode.Ignore)]
        public IIconModel Icon => this.GetApplicationTemplateSettings().Icon();

        [IntentManaged(Mode.Ignore)]
        public string DefaultName => this.GetApplicationTemplateSettings().DefaultName() ?? "NewApplication";

        [IntentManaged(Mode.Ignore)]
        public string DefaultOutputLocation => this.GetApplicationTemplateSettings().DefaultOutputLocation() ?? "";
    }
}