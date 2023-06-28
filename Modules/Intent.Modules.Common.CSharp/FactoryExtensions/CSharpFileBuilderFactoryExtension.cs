using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;

namespace Intent.Modules.Common.Plugins
{
    [Description("Intent.Common.TemplateLifeCycleHooks")]
    public class CSharpFileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.CSharp.CSharpFileBuilderFactoryExtension";

        public override int Order => 100000000; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.CSharpFile.StartBuild());
            templates.ForEach(x => x.CSharpFile.CompleteBuild());
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.CSharpFile.AfterBuild());
        }
    }
}