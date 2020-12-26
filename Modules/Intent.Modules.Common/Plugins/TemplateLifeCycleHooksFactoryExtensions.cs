using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;

namespace Intent.Modules.Common.Plugins
{
    [Description("Intent.Common.TemplateLifeCycleHooks")]
    public class TemplateLifeCycleHooksFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle, ITemplateLifeCycle
    {
        public override string Id => "Intent.Common.TemplateLifeCycleHooks";

        public override int Order => -1;

        public void OnStep(IApplication application, string step)
        {
            // TODO: Run for decorators too. Probably as separate FactoryExtension
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                var templates = application.Projects.SelectMany(x => x.TemplateInstances)
                    .Where(x => x is ITemplateBeforeExecutionHook)
                    .Cast<ITemplateBeforeExecutionHook>()
                    .ToList();

                templates.ForEach(x => x.BeforeTemplateExecution());
            }
        }

        public void PostConfiguration(ITemplate template)
        {
            var t = template as ITemplatePostConfigurationHook;
            t?.OnConfigured();
        }

        public void PostCreation(ITemplate template)
        {
            var t = template as ITemplatePostCreationHook;
            t?.OnCreated();
        }
    }
}