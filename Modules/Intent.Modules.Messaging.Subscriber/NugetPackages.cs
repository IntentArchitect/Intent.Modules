using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Messaging.Subscriber
{
    public static class NugetPackages
    {
        public static NugetPackageInfo AkkaRemote = new NugetPackageInfo("Akka.Remote", "1.1.1", "net45");

        public static NugetPackageInfo IntentEsbClient = new NugetPackageInfo("Intent.Esb.Client", "0.1.14-beta", "net45");

        public static NugetPackageInfo IntentEsbServer = new NugetPackageInfo("Intent.Esb.Server", "0.1.14-beta", "net45");
    }
}