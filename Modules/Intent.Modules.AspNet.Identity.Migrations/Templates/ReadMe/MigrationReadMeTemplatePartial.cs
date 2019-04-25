﻿using Intent.Modules.AspNet.Identity.Migrations.Templates.DbMigrationsConfiguration;
using Intent.Engine;
using Intent.Templates
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AspNet.Identity.Migrations.Templates.ReadMe
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

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "MIGRATION_README",
                fileExtension: "txt",
                defaultLocationInProject: "Identity"
                );
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(IdentityDbMigrationsConfigurationTemplate.Identifier)
            };
        }
    }
}
