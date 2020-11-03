using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("IdentityServer4.Selfhost.Program", Version = "1.0")]

namespace IdentityServer4StandaloneApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}