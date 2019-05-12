using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;

namespace Intent.Modules.EntityFramework.Interop.Unity.Decorators
{
    public class EntityFrameworkUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFramework.Interop.Unity";

        private readonly IApplication _application;

        public EntityFrameworkUnityRegistrationsDecorator(IApplication application)
        {
            _application = application;
        }

        public IEnumerable<string> DeclareUsings() => new [] {
            "Intent.Framework.EntityFramework",
        };

        public string Registrations() => $@"
            container.RegisterType<IDbContextFactory, DbContextFactory>();
";
        //public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        //{
        //    return new[]
        //    {
        //        TemplateDependency.OnTemplate(DeleteVisitorTemplate.Identifier)
        //    };
        //}

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>
            {
                new NugetPackageInfo("EntityFramework", "6.2.0"),
                new NugetPackageInfo("Intent.Framework.EntityFramework", "1.0.0")
            };
        }

        public int Priority { get; } = 0;
    }
}