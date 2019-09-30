using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.DbContext
{
    partial class DbContextTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<IClass>>, ITemplate, IHasNugetDependencies, ITemplateBeforeExecutionHook, IHasDecorators<DbContextDecoratorBase>
    {
        public const string Identifier = "Intent.EntityFramework.DbContext";

        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IList<DbContextDecoratorBase> _decorators = new List<DbContextDecoratorBase>();

        public DbContextTemplate(IEnumerable<IClass> models, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, models)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }


        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Project.Application.Name}DbContext".AsClassName(),
                fileExtension: "cs",
                defaultLocationInProject: "DbContext",
                className: $"{Project.Application.Name}DbContext".AsClassName(),
                @namespace: "${Project.ProjectName}"
                );
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

        public override void BeforeTemplateExecution()
        {
            _eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            {
                { "Name", $"{Project.Application.Name}DB" },
                { "ConnectionString", $"Server=.;Initial Catalog={ Project.Application.SolutionName };Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });

            _eventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "ConcreteTypeTemplateId", Identifier },
                { "Lifetime", ContainerRegistrationEvent.PerServiceCallLifetime }
            });
        }

        public void AddDecorator(DbContextDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DbContextDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string GetBaseClass()
        {
            try
            {
                return GetDecorators().Select(x => x.GetBaseClass()).SingleOrDefault(x => x != null) ?? "DbContext";
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {Identifier}");
            }
        }
    }
}
