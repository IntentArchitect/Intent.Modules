using System.Collections.Generic;
using System.Linq;
using Intent.Packages.AspNet.Identity.Migrations.Templates.DbMigrationsConfiguration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.AspNet.Identity.Migrations.Templates.ReadMe
{
    partial class MigrationReadMeTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.Identity.Migrations.ReadMe";


        public MigrationReadMeTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string ProjectWithDbContext => Project.Application.Projects.FirstOrDefault(x => x.HasStereotype("Startup"))?.Name ?? Project.Application.Projects.First().Name;
        public string DbContextConfigurationName => Project.FindTemplateInstance(IdentityDbMigrationsConfigurationTemplate.Identifier).GetMetaData().FileName;

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                defaultLocationInProject: "Identity"
                );
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(IdentityDbMigrationsConfigurationTemplate.Identifier)
            };
        }
    }
}
