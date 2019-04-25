using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration;
using Intent.Engine;
using Intent.Templates

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsReadMe
{
    partial class DbMigrationsReadMeTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFramework.DbMigrationsReadMe";


        public DbMigrationsReadMeTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string ProjectWithDbContext => Project.Application.Projects.FirstOrDefault(x => x.HasStereotype("Startup"))?.Name ?? Project.Application.Projects.First().Name;
        public string DbContextConfigurationName => Project.FindTemplateInstance(DbMigrationsConfigurationTemplate.Identifier).GetMetaData().FileName;

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                defaultLocationInProject: ""
                );
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(DbMigrationsConfigurationTemplate.Identifier)
            };
        }
    }
}
