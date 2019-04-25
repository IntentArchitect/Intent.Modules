using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

namespace Intent.Modules.EntityFrameworkCore.Templates.DbMigrationsReadMe
{
    partial class DbMigrationsReadMeTemplate : IntentProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbMigrationsReadMe";


        public DbMigrationsReadMeTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public string BoundedContextName => Project.ApplicationName();
        public string MigrationProject => Project.Name;
        public string ProjectWithDbContext => Project.Application.Projects.FirstOrDefault(x => x.HasStereotype("Startup"))?.Name ?? Project.Application.Projects.First().Name;

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                defaultLocationInProject: ""
                );
        }
    }
}
