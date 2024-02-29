using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.TypeScript.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TypeScriptFileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.TypeScript.TypeScriptFileBuilderFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => int.MaxValue; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .OfType<ITypescriptFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.TypescriptFile.StartBuild());
            templates.ForEach(x => x.TypescriptFile.CompleteBuild());
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .OfType<ITypescriptFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x => x.TypescriptFile.AfterBuild());
        }
    }
}