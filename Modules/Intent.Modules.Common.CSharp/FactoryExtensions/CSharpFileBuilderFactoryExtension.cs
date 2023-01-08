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

        public override int Order => int.MaxValue; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.CSharpFile.StartBuild());
            templates.ForEach(x => x.CSharpFile.CompleteBuild());
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .OfType<ICSharpFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.CSharpFile.AfterBuild());
            templates.ForEach(x => x.CSharpFile.AfterBuild()); // TODO JL: Why is this being called twice?
        }
    }
}