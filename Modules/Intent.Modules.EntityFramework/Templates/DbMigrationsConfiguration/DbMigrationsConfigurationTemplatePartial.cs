using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Engine;
using Intent.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration
{
    partial class DbMigrationsConfigurationTemplate : IntentRoslynProjectItemTemplateBase, ITemplate, IHasNugetDependencies, IHasTemplateDependencies, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFramework.DbMigrationsConfiguration";
        private ITemplateDependency _dbContextDependancy;
        private readonly List<string> _seedDataRequiredEvents = new List<string>();

        public DbMigrationsConfigurationTemplate(IProject project)
            : base(Identifier, project)
        {
            project.Application.EventDispatcher.Subscribe(EntityFrameworkEvents.SeedDataRequiredEvent, Handle);
        }

        private void Handle(ApplicationEvent @event)
        {
            _seedDataRequiredEvents.Add(@event.AdditionalInfo[EntityFrameworkEvents.SeedDataRequiredEventKey]);
        }

        public string DbContextVariableName => "dbContext";

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Project.ApplicationName()}DbContextConfiguration".Replace(".", string.Empty),
                fileExtension: "cs",
                defaultLocationInProject: "",
                className: $"{Project.ApplicationName()}DbContextConfiguration".Replace(".", string.Empty),
                @namespace: "${Project.Name}"
                );
        }

        private string[] GetSeedDataRequiredRegistrations()
        {
            return _seedDataRequiredEvents.ToArray();
        }

        public void Created()
        {
            var fileMetadata = GetMetadata();
            _dbContextDependancy = TemplateDependency.OnTemplate(DbContextTemplate.Identifier);
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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _dbContextDependancy,
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

    }
}
