using Intent.Modules.Auditing.Templates.ServiceBoundaryAudtingStrategy;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Auditing.Decorators
{
    public class AuditingUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Auditing.Unity.Auditing.Decorator";
        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using Intent.Framework.EntityFramework",
                "using Intent.Framework.EntityFramework.Auditing;",
            };
        }

        public string Registrations() => @"
            container.RegisterType<IServiceBoundaryAuditingStrategy, ServiceBoundaryAuditingStrategy>();
            container.RegisterType<IDbContextSaveInterceptor, AuditDbContextSaveInterceptor>(nameof(AuditDbContextSaveInterceptor));
";

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(ServiceBoundaryAuditingStrategyTemplate.Identifier),
            };
        }
        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            };
        }
    }
}
