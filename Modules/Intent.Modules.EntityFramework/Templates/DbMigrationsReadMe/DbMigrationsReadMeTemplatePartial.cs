using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration;
using Intent.Engine;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsReadMe
{
    partial class DbMigrationsReadMeTemplate : IntentFileTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFramework.DbMigrationsReadMe";


        public DbMigrationsReadMeTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string StartupProject => OutputTarget.Application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.WebApiApplication || x.Type == VisualStudioProjectTypeIds.ConsoleAppNetFramework)?.Name ?? "Unknown";
        public string DbContextConfigurationName => ExecutionContext.FindTemplateInstance(DbMigrationsConfigurationTemplate.Identifier).GetMetadata().FileName;

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                relativeLocation: ""
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
