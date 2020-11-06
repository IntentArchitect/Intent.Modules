using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbMigrationsConfiguration
{
    partial class DbMigrationsConfigurationTemplate : CSharpTemplateBase, ITemplate, IHasNugetDependencies, IHasTemplateDependencies, ITemplatePostCreationHook
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

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Project.ApplicationName()}DbContextConfiguration".ToCSharpIdentifier(),
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        private string[] GetSeedDataRequiredRegistrations()
        {
            return _seedDataRequiredEvents.ToArray();
        }

        public override void OnCreated()
        {
            var fileMetadata = GetMetadata();
            _dbContextDependancy = TemplateDependency.OnTemplate(DbContextTemplate.Identifier);
        }

        public string GetDbContextClassName()
        {
            var dbContext = this.Project.FindTemplateInstance<IClassProvider>(_dbContextDependancy);
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

    }
}
