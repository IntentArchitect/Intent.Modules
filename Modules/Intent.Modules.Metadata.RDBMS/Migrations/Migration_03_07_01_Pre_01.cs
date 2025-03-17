using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.RDBMS.Migrations
{
    public class Migration_03_07_01_Pre_01 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_07_01_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.RDBMS";
        [IntentFully]
        public string ModuleVersion => "3.7.1-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.DomainDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                if (package.Stereotypes.Count > 0 ||
                    package.Metadata.Any(x => x.Key == "database-paradigm-selected" && x.Value == "true"))
                {
                    continue;
                }

                package.Stereotypes.Add(new StereotypePersistable
                {
                    DefinitionId = DomainPackageModelStereotypeExtensions.RelationalDatabase.DefinitionId,
                    Name = "Relational Database",
                    DefinitionPackageName = "Intent.Metadata.RDBMS",
                    DefinitionPackageId = "AF8F3810-745C-42A2-93C8-798860DC45B1"
                });

                package.Save(true);
            }
        }

        public void Down()
        {
        }
    }
}