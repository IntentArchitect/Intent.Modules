using Intent.Modules.AspNet.WebApi.Legacy;
using Intent.SoftwareFactory.MetaModels.Service;

namespace Intent.Modules.AspNet.WebApi.Decorators.Legacy
{
    public class DebugSleepDistributionDecorator : BaseDistributionDecorator
    {
        public const string Identifier = "Intent.WebApi.Distribution.DebugSleep.Decorator.Legacy";
        private readonly int _sleepTimeInMilliseconds;

        public DebugSleepDistributionDecorator(int sleepTimeInMilliseconds)
        {
            _sleepTimeInMilliseconds = sleepTimeInMilliseconds;
        }

        public override string BeginOperation(ServiceModel service, ServiceOperationModel operation) => $@"
#if DEBUG
                System.Threading.Thread.Sleep({ _sleepTimeInMilliseconds });
#endif";
    }
}