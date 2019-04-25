using Intent.Modules.IdentityServer.Templates.AspNetIdentityModel;
using Intent.Engine;
using Intent.Templates
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.Identity.Migrations.Templates.DbMigrationsConfiguration
{
    partial class IdentityDbMigrationsConfigurationTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.Identity.Migrations.DbMigrationsConfiguration";

        public IdentityDbMigrationsConfigurationTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IdentityDbContextConfiguration",
                fileExtension: "cs",
                defaultLocationInProject: "Identity",
                className: "IdentityDbContextConfiguration",
                @namespace: "${Project.Name}.Identity"
                );
        }

        public string DbContextClassName => AspNetIdentityModelTemplate.DB_CONTEXT_NAME;

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFramework,
                NugetPackages.MicrosoftAspNetIdentityEntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new []
            {
                TemplateDependency.OnTemplate(AspNetIdentityModelTemplate.Identifier),
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }
    }
}
