using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Service;
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

        public override string DeclarePrivateVariables(IServiceModel service) => HasBusinessQueueNeed(service) ? @"
        private readonly IBusinessQueueInternals _businessQueue;" : string.Empty;

        public override string ConstructorParams(IServiceModel service) => HasBusinessQueueNeed(service) ? @"
            , IBusinessQueue businessQueue" : string.Empty;

        public override string ConstructorInit(IServiceModel service) => HasBusinessQueueNeed(service) ? @"
            _businessQueue = (IBusinessQueueInternals)businessQueue;" : string.Empty;

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => ShouldProcessQueue(operation) ? @"
                    _businessQueue.Flush();" : "";

        public override string AfterTransaction(IServiceModel service, IOperationModel operation) => ShouldProcessQueue(operation) ? @"
                _businessQueue.NotifyQueueProcessors();" : "";

        public override int Priority { get; set; } = -300;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.AkkaRemote,
                NugetPackages.AkkaLoggerNLog,
                NugetPackages.IntentEsbClient,
            };
        }

        private bool HasBusinessQueueNeed(IServiceModel service)
        {
            return service.Operations.Any(x => x.GetStereotypeProperty<bool>("Business Queue", "Explicit Queue Processing", true));
        }

        private bool ShouldProcessQueue(IOperationModel operation)
        {
            return operation.GetStereotypeProperty<bool>("Business Queue", "Explicit Queue Processing", true)
                && !operation.GetStereotypeProperty<bool>("Business Queue", "Read Only");
        }
    }
}