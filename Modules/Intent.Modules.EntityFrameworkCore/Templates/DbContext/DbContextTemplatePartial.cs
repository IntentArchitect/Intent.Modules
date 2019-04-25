using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.Templates

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    partial class DbContextTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<IClass>>, IBeforeTemplateExecutionHook, IHasDecorators<DbContextDecoratorBase>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbContext";

        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IEnumerable<DbContextDecoratorBase> _decorators;

        public DbContextTemplate(IEnumerable<IClass> models, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, models)
        {
            _eventDispatcher = eventDispatcher;
        }

        public string GetEntityName(IClass model)
        {
            var template = Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == model.Id));
            return template != null ? NormalizeNamespace($"{template.ClassName}") : $"{model.Name}";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Project.Application.ApplicationName}DbContext".AsClassName(),
                fileExtension: "cs",
                defaultLocationInProject: "DbContext",
                className: $"{Project.Application.ApplicationName}DbContext".AsClassName(),
                @namespace: "${Project.ProjectName}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return (UseLazyLoadingProxies
                ? new[]
                {
                    NugetPackages.EntityFrameworkCore,
                    NugetPackages.EntityFrameworkCoreProxies,
                }
                : new[]
                {
                    NugetPackages.EntityFrameworkCore,
                })
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void BeforeTemplateExecution()
        {
            _eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            {
                { "Name", $"{Project.Application.ApplicationName}DB" },
                { "ConnectionString", $"Server=.;Initial Catalog={Project.Application.SolutionName}.{ Project.Application.ApplicationName };Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });

            _eventDispatcher.Publish(ContainerRegistrationForDbContextEvent.EventId, new Dictionary<string, string>()
            {
                { ContainerRegistrationForDbContextEvent.UsingsKey, $"Microsoft.EntityFrameworkCore;" },
                { ContainerRegistrationForDbContextEvent.ConcreteTypeKey, $"{Namespace}.{ClassName}" },
                { ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey, Identifier },
                { ContainerRegistrationForDbContextEvent.OptionsKey, $@".{GetDbContextDbServerSetupMethodName()}(Configuration.GetConnectionString(""{Project.Application.ApplicationName}DB"")){(UseLazyLoadingProxies ? ".UseLazyLoadingProxies()" : "")}" },
            });
        }

        public bool UseLazyLoadingProxies => !bool.TryParse(GetMetaData().CustomMetaData["Use Lazy-Loading Proxies"], out var useLazyLoadingProxies) || useLazyLoadingProxies;

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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return Model.Select(x => TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == x.Id)).ToList();
        }

        private string GetDbContextDbServerSetupMethodName()
        {
            var dbContextDbServerName = GetMetaData().CustomMetaData["DbContext DbServer Setup"];
            return dbContextDbServerName;
        }
    }
}
