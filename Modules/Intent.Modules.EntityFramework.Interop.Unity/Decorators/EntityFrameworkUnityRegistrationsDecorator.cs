using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.RichDomain.EntityFramework.Templates.DeleteVisitor;

namespace Intent.Modules.EntityFramework.Interop.Unity.Decorators
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