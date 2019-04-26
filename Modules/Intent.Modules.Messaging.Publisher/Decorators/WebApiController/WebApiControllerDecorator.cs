using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Messaging.Publisher.Decorators.WebApiController
{
    public class WebApiControllerDecorator : WebApiControllerDecoratorBase, IHasNugetDependencies
    {
        public const string IDENTIFIER = "Intent.Messaging.Publisher.WebApiControllerDecorator";

        public override IEnumerable<string> DeclareUsings() => new List<string>
        {
            "Intent.Esb.Client.Publishing",
        };

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private readonly IBusinessQueueInternals _businessQueue;";

        public override string ConstructorParams(IServiceModel service) => @"
            , IBusinessQueue businessQueue";

        public override string ConstructorInit(IServiceModel service) => @"
            _businessQueue = (IBusinessQueueInternals)businessQueue;";

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => !operation.HasStereotype("ReadOnly") ? @"
                    _businessQueue.Flush();" : "";

        public override string AfterTransaction(IServiceModel service, IOperationModel operation) => !operation.HasStereotype("ReadOnly") ? @"
                _businessQueue.NotifyQueueProcessors();" : "";

        public override int Priority { get; set; } = -300;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentEsbClient,
            };
        }
    }
}