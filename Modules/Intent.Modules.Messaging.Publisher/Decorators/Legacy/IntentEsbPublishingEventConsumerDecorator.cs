using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Messaging.Publisher.Decorators.Legacy
{
    // This is almost an exact copy of IntentEsbPublishingDistributionDecorator
    public class IntentEsbPublishingEventConsumerDecorator : IEventConsumerDecorator
    {
        public const string Identifier = "Intent.Messaging.Publisher.EventConsumerDecorator.Legacy";

        public IEnumerable<string> DeclareUsings() => new[]
        {
            "using Intent.Esb.Client.Publishing;",
        };

        public string DeclarePrivateVariables() => @"
        private readonly IBusinessQueueInternals _businessQueue;";

        public string ConstructorParams() => @"
            , IBusinessQueue businessQueue";

        public string ConstructorInit() => @"
            _businessQueue = (IBusinessQueueInternals)businessQueue;";
        public string BeginOperation() => @"";

        public string BeforeTransaction() => @"";

        public string BeforeCallToAppLayer() => @"";

        public string AfterCallToAppLayer() => @"
                        _businessQueue.Flush();";

        public string AfterTransaction() => @"
                _businessQueue.NotifyQueueProcessors();";

        public string OnExceptionCaught() => @"";

        public bool HandlesCaughtException() => false;

        public string ClassMethods() => @"";

        public int Priority { get; set; } = -300;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.AkkaRemote,
                NugetPackages.IntentEsbClient,
            };
        }

    }
}