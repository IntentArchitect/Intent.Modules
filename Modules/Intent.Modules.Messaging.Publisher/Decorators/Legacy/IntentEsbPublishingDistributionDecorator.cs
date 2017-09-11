using System.Collections.Generic;
using Intent.Modules.WebApi.Legacy;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Messaging.Publisher.Decorators.Legacy
{
    public class IntentEsbPublishingDistributionDecorator : BaseDistributionDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Messaging.Publisher.DistributionDecorator.Legacy";

        public override IEnumerable<string> DeclareUsings(ServiceModel service) => new List<string>
        {
            "using Intent.Esb.Client.Publishing;",
        };

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IBusinessQueueInternals _businessQueue;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IBusinessQueue businessQueue";

        public override string ConstructorInit(ServiceModel service) => @"
            _businessQueue = (IBusinessQueueInternals)businessQueue;";

        public override string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => !operation.IsReadOnly ? @"
                    _businessQueue.Flush();" : "";

        public override string AfterTransaction(ServiceModel service, ServiceOperationModel operation) => !operation.IsReadOnly ? @"
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
    }
}