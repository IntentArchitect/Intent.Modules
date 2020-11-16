using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
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

        private readonly IList<DbContextDecoratorBase> _decorators = new List<DbContextDecoratorBase>();

        public DbContextTemplate(IEnumerable<ClassModel> models, IOutputTarget outputTarget)
            : base(Identifier, outputTarget, models)
        {
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(GetMetadata().CustomMetadata["Entity Template Id"], model);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Project.Application.Name}DbContext".ToCSharpIdentifier(),
                @namespace: $"{OutputTarget.GetNamespace()}");
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
                return NormalizeNamespace(GetDecorators().Select(x => x.GetBaseClass()).SingleOrDefault(x => x != null) ?? "Microsoft.EntityFrameworkCore.DbContext");
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {Identifier}");
            }
        }

        public string GetMappingClassName(ClassModel model)
        {
            return GetTypeName(EFMapping.EFMappingTemplate.Identifier, model);
        }
    }
}
