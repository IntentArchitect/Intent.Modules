using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.Startup;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    public class SwashbuckleCoreWebStartupDecorator : CoreWebStartupDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.AspNetCore.Swashbuckle.StartupDecorator";
        public SwashbuckleCoreWebStartupDecorator()
        {
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.SwashbuckleAspNetCore,
            };
        }
    }
}
