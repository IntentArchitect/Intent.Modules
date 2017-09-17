using Intent.MetaModel.Service;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    // TODO: This should move out to its own package.
    public class WebApiDistributionExceptionHandlingStrategy : DistributionDecoratorBase, IHasNugetDependencies
    {
        public const string Identifier = "Intent.WebApi.ExceptionHandlingStrategy";

        public override IEnumerable<string> DeclareUsings(IServiceModel service) => new List<string>
        {
            "using Intent.Framework.WebApi.ExceptionHandling;"
        };

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private readonly IServiceBoundaryExceptionHandlingStrategy _exceptionHandlingStrategy;";

        public override string ConstructorParams(IServiceModel service) => @"
            , IServiceBoundaryExceptionHandlingStrategy exceptionHandlingStrategy";

        public override string ConstructorInit(IServiceModel service) => @"
            _exceptionHandlingStrategy = exceptionHandlingStrategy;";

        public override string OnExceptionCaught(IServiceModel service, IOperationModel operation) => @"
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
