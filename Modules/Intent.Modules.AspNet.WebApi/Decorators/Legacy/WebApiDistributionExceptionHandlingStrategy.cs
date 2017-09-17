using Intent.Modules.AspNet.WebApi.Legacy;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Decorators.Legacy
{
    // TODO: This should move out to its own package.
    public class WebApiDistributionExceptionHandlingStrategy : BaseDistributionDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.WebApi.ExceptionHandlingStrategy.Legacy";

        public override IEnumerable<string> DeclareUsings(ServiceModel service) => new List<string>
        {
            "using Intent.Framework.WebApi.ExceptionHandling;"
        };

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IServiceBoundaryExceptionHandlingStrategy _exceptionHandlingStrategy;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IServiceBoundaryExceptionHandlingStrategy exceptionHandlingStrategy";

        public override string ConstructorInit(ServiceModel service) => @"
            _exceptionHandlingStrategy = exceptionHandlingStrategy;";

        public override string OnExceptionCaught(ServiceModel service, ServiceOperationModel operation) => @"
                _exceptionHandlingStrategy.HandleException(e);";

        public override bool HandlesCaughtException() => true;

        public override int Priority { get; set; } = -400;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkWebApi,
            };
        }
    }
}
