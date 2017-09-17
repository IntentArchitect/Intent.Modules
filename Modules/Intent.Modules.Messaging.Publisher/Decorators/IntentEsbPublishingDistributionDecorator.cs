using Intent.MetaModel.Service;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Messaging.Publisher.Decorators
{
    public class IntentEsbPublishingDistributionDecorator : DistributionDecoratorBase, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Messaging.Publisher.DistributionDecorator";

        public override IEnumerable<string> DeclareUsings(IServiceModel service) => new List<string>
        {
            "using Intent.Esb.Client.Publishing;",
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
                NugetPackages.AkkaRemote,
                NugetPackages.AkkaLoggerNLog,
                NugetPackages.IntentEsbClient,
            };
        }
    }
}