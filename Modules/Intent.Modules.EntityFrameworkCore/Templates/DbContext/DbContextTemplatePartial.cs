using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContext
{
    partial class DbContextTemplate : CSharpTemplateBase<IEnumerable<ClassModel>>, ITemplateBeforeExecutionHook, IHasDecorators<DbContextDecoratorBase>, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFrameworkCore.DbContext";

        private readonly IApplicationEventDispatcher _eventDispatcher;
        private IList<DbContextDecoratorBase> _decorators = new List<DbContextDecoratorBase>();

        public DbContextTemplate(IEnumerable<ClassModel> models, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, models)
        {
            _eventDispatcher = eventDispatcher;
        }

        public string GetEntityName(ClassModel model)
        {
            var template = Project.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == model.Id));
            return template != null ? NormalizeNamespace($"{template.ClassName}") : $"{model.Name}";
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"{Project.Application.Name}DbContext".AsClassName(),
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        //protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        //{
        //    return new RoslynDefaultFileMetadata(
        //        overwriteBehaviour: OverwriteBehaviour.Always,
        //        fileName: $"{Project.Application.Name}DbContext".AsClassName(),
        //        fileExtension: "cs",
        //        defaultLocationInProject: "DbContext",
        //        className: $"{Project.Application.Name}DbContext".AsClassName(),
        //        @namespace: "${Project.ProjectName}"
        //        );
        //}

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
            //_eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            //{
            //    { "Name", $"{Project.Application.Name}DB" },
            //    { "ConnectionString", $"Server=.;Initial Catalog={Project.Application.SolutionName}.{ Project.Application.Name };Integrated Security=true;MultipleActiveResultSets=True" },
            //    { "ProviderName", "System.Data.SqlClient" },
            //});

            //_eventDispatcher.Publish(ContainerRegistrationForDbContextEvent.EventId, new Dictionary<string, string>()
            //{
            //    { ContainerRegistrationForDbContextEvent.UsingsKey, $"Microsoft.EntityFrameworkCore;" },
            //    { ContainerRegistrationForDbContextEvent.ConcreteTypeKey, $"{Namespace}.{ClassName}" },
            //    { ContainerRegistrationForDbContextEvent.ConcreteTypeTemplateIdKey, Identifier },
            //    { ContainerRegistrationForDbContextEvent.OptionsKey, $@".{GetDbContextDbServerSetupMethodName()}(Configuration.GetConnectionString(""{Project.Application.Name}DB"")){(UseLazyLoadingProxies ? ".UseLazyLoadingProxies()" : "")}" },
            //    { ContainerRegistrationForDbContextEvent.NugetDependency, NugetPackages.EntityFrameworkCoreSqlServer. },
            //    { ContainerRegistrationForDbContextEvent.NugetDependencyVersion, "2.1.1" },
            //});
        }

        public bool UseLazyLoadingProxies => !bool.TryParse(GetMetadata().CustomMetadata["Use Lazy-Loading Proxies"], out var useLazyLoadingProxies) || useLazyLoadingProxies;

        public void AddDecorator(DbContextDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DbContextDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string DeclareUsings()
        {
            return string.Join(Environment.NewLine, GetDecorators().SelectMany(x => x.DeclareUsings()).Select(s => $"using {s};"));
        }

        public string GetMethods()
        {
            var code = string.Join(Environment.NewLine + Environment.NewLine,
                GetDecorators()
                    .SelectMany(s => s.GetMethods())
                    .Where(p => !string.IsNullOrEmpty(p)));
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }
            return Environment.NewLine + Environment.NewLine + code;
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
            return Model.Select(x => TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == x.Id)).ToList();
        }
    }
}
