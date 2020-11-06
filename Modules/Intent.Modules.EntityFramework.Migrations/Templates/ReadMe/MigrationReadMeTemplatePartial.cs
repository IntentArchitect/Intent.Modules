using Intent.Modules.EntityFramework.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.EntityFramework.Migrations.Templates.ReadMe
{
    partial class MigrationReadMeTemplate : IntentFileTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFramework.Migrations.ReadMe";


        public MigrationReadMeTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string StartupProject => OutputTarget.Application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.WebApiApplication || x.Type == VisualStudioProjectTypeIds.ConsoleAppNetFramework)?.Name ?? "Unknown";
        public string DbContextConfigurationName => Project.FindTemplateInstance(DbMigrationsConfigurationTemplate.Identifier).GetMetadata().FileName;

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
