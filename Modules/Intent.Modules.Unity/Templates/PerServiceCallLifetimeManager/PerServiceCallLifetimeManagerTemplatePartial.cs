using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager
{
    partial class PerServiceCallLifetimeManagerTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Unity.PerServiceCallLifetimeManager";

        public PerServiceCallLifetimeManagerTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"PerServiceCallLifetimeManager",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkCore
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
