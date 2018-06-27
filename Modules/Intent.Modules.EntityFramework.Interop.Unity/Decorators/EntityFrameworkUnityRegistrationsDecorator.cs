using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.VisualStudio;

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
        };

        // TODO: should use template lookup for DbContext name.
        public string Registrations() => $@"
            container.RegisterType<{_application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(DbContextTemplate.Identifier)).ClassName}>(new PerServiceCallLifetimeManager());";

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DbContextTemplate.Identifier)
            };
        }
    }
}