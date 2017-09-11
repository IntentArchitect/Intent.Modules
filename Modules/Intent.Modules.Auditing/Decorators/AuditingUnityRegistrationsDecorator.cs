using System.Collections.Generic;
using Intent.Packages.Auditing.Templates.ServiceBoundaryAudtingStrategy;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Auditing.Decorators
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
