using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware
{
    public partial class RequireHttpsMiddlewareTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.AspNet.WebApi.RequireHttpsMiddleware";
        public RequireHttpsMiddlewareTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"RequireHttpsMiddleware",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftOwin,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
