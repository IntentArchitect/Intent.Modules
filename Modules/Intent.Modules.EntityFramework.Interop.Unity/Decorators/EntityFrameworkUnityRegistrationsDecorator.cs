using System.Collections.Generic;
using Intent.Packages.EntityFramework.Templates.DeleteVisitor;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.EntityFramework.Interop.Unity.Decorators
{
    public class EntityFrameworkUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFramework.Interop.Unity";

        private readonly IApplication _application;

        public EntityFrameworkUnityRegistrationsDecorator(IApplication application)
        {
            _application = application;
        }

        public IEnumerable<string> DeclareUsings() => new [] {
            "using Intent.Framework.EntityFramework;",
            "using Intent.Framework.EntityFramework.Interceptors;"
        };

        public string Registrations() => $@"
            container.RegisterType<IDbContextFactory, DbContextFactory>();
            container.RegisterType<IDbContextSaveInterceptor, OrphanDeleteInterceptor>(""OrphanDeleteInterceptor"");
            container.RegisterType<ICompositionDeleteVisitor, { _application.ApplicationName }DeleteVisitor>();
";
        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DeleteVisitorTemplate.Identifier)
            };
        }
    }
}