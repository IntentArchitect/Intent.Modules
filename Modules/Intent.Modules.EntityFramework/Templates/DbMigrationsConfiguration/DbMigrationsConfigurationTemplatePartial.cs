using System.Collections.Generic;
using System.Linq;
using Intent.Modules.EF;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration
{
    partial class DbMigrationsConfigurationTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies, IHasTemplateDependencies, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFramework.DbMigrationsConfiguration";
        private ITemplateDependancy _dbContextDependancy;

        public DbMigrationsConfigurationTemplate(IProject project)
            : base(Identifier, project)
        {
        }

        public string DbContextVariableName => "dbContext";

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Project.ApplicationName()}DbContextConfiguration".Replace(".", string.Empty),
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: $"{Project.ApplicationName()}DbContextConfiguration".Replace(".", string.Empty),
                @namespace: "${Project.Name}"
                );
        }

        public void Created()
        {
            var fileMetaData = GetMetaData();
            _dbContextDependancy = TemplateDependancy.OnTemplate(DbContextTemplate.Identifier);
        }

        public string GetDbContextClassName()
        {
            var dbContext = this.Project.FindTemplateInstance<IHasClassDetails>(_dbContextDependancy);
            return dbContext.ClassName;
        }       

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                _dbContextDependancy,
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

    }
}
