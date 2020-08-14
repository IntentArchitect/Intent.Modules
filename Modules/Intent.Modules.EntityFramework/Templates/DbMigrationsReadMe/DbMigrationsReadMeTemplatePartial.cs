using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration;
using Intent.Engine;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Templates;

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
        public string ProjectWithDbContext => ExecutionContext.FindOutputTargetWithTemplateInstance(TemplateDependency.OnTemplate(DbContextTemplate.Identifier))?.Name ?? "<UNKNOWN-DB-CONTEXT-PROJECT>";
        public string DbContextConfigurationName => ExecutionContext.FindTemplateInstance(DbMigrationsConfigurationTemplate.Identifier).GetMetadata().FileName;

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
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
