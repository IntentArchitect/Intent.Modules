using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Messaging.Publisher
{
    public static class NugetPackages
    {
        public static NugetPackageInfo AkkaRemote = new NugetPackageInfo("Akka.Remote", "1.1.1", "net45");
        public static NugetPackageInfo AkkaLoggerNLog = new NugetPackageInfo("Akka.Logger.NLog", "1.1.1", "net45");
        public static NugetPackageInfo IntentEsbClient = new NugetPackageInfo("Intent.Esb.Client", "0.1.17-beta", "net45");
    }
}