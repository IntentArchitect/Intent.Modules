using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    partial class DbContextTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<IClass>>, ITemplate, IHasNugetDependencies, IBeforeTemplateExecutionHook, IHasDecorators<DbContextDecoratorBase>
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbContext";

        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IEnumerable<DbContextDecoratorBase> _decorators;

        public DbContextTemplate(IEnumerable<IClass> models, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, models)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Project.Application.ApplicationName}DbContext",
                fileExtension: "cs",
                defaultLocationInProject: "DbContext",
                className: "${Project.Application.ApplicationName}DbContext",
                @namespace: "${Project.ProjectName}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFrameworkCore,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void BeforeTemplateExecution()
        {
            _eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            {
                { "Name", $"{Project.Application.ApplicationName}DB" },
                { "ConnectionString", $"Server=.;Initial Catalog={ Project.Application.SolutionName };Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });

            _eventDispatcher.Publish(ContainerRegistrationForDbContextEvent.EventId, new Dictionary<string, string>()
            {
                { ContainerRegistrationForDbContextEvent.ConcreteTypeKey, $"{Namespace}.{ClassName}" },
                { ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey, Identifier },
                { ContainerRegistrationForDbContextEvent.OptionsKey, $@".UseSqlServer(Configuration.GetConnectionString(""{Project.Application.ApplicationName}DB"")).UseLazyLoadingProxies()" },
            });
        }

        public IEnumerable<DbContextDecoratorBase> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
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
