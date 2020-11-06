using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    partial class DbMigrationsReadMeTemplate : IntentFileTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbMigrationsReadMe";


        public DbMigrationsReadMeTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string StartupProject => OutputTarget.Application.OutputTargets.FirstOrDefault(x => x.Type == VisualStudioProjectTypeIds.CoreWebApp)?.Name ?? "UNKNOWN";
        //public string ProjectWithDbContext => ExecutionContext.FindOutputTargetWithTemplateInstance(TemplateDependency.OnTemplate(DbContextTemplate.Identifier))?.Name ?? "<UNKNOWN-DB-CONTEXT-PROJECT>";

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                relativeLocation: "");
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFrameworkCoreDesign,
                NugetPackages.EntityFrameworkCoreTools,
            }
            .ToArray();
        }
    }
}
