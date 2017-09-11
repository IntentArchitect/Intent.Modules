using System.Collections.Generic;
using System.Linq;
using Intent.Modules.WebApi.Legacy;
using Intent.Packages.Auditing.Templates.HttpRequestMessageExtensions;
using Intent.Packages.Auditing.Templates.ServiceBoundaryAudtingStrategy;
using Intent.Packages.UserContext.Templates.UserContextInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Auditing.Decorators
{
    public class AuditingDistributionDecorator : BaseDistributionDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Auditing.Distribution.Auditing.Decorator";
        private readonly IApplication _applicationManager;

        public AuditingDistributionDecorator(IApplication applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public override IEnumerable<string> DeclareUsings(ServiceModel service)
        {
            if (service.Operations.All(x => x.IsReadOnly))
            {
                return new string[0];
            }

            return new[]
            {
                "using Intent.Framework.EntityFramework.Auditing;"
            };
        }

        public override string DeclarePrivateVariables(ServiceModel service) => service.Operations.All(x => x.IsReadOnly) ? string.Empty : @"
        private readonly IServiceBoundaryAuditingStrategy _auditingStrategy;";

        public override string ConstructorParams(ServiceModel service) => service.Operations.All(x => x.IsReadOnly) ? string.Empty : @"
            , IServiceBoundaryAuditingStrategy  auditingStrategy";

        public override string ConstructorInit(ServiceModel service) => service.Operations.All(x => x.IsReadOnly) ? string.Empty : @"
            _auditingStrategy = auditingStrategy;";

        public override string BeginOperation(ServiceModel service, ServiceOperationModel operation) => operation.IsReadOnly ? string.Empty : $@"
             AuditContext.Current.StartAuditing(
                userId: ServiceCallContext.Instance.Get<IUserContextData>().UserId,
                application: ""{_applicationManager.ApplicationName}"",
                service: ""{service.Name}"",
                serviceOperation: ""{operation.Name}"",
                ipAddress: Request.GetClientIpAddress());
            _auditingStrategy.BeforeServiceCallHandler(AuditContext.Current.ServiceCallAudit);";

        public override string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => operation.IsReadOnly ? string.Empty : @"
                        _auditingStrategy.AfterServiceCallHandler(AuditContext.Current.ServiceCallAudit);";

        public override int Priority { get; set; } = -300;

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UserContextInterfaceTemplate.Identifier),
                TemplateDependancy.OnTemplate(ServiceBoundaryAuditingStrategyTemplate.Identifier),
                TemplateDependancy.OnTemplate(HttpRequestMessageExtensionsTemplate.Identifier),
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
