using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;

namespace Intent.Modules.Common.Plugins
{
    [Description("Intent.Common.TemplateLifeCycleHooks")]
    public class TemplateLifeCycleHooksFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.Common.TemplateLifeCycleHooks";

        public override int Order => -1;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                var templates = application.Projects.SelectMany(x => x.TemplateInstances)
                    .Where(x => x is IBeforeTemplateExecutionHook)
                    .Cast<IBeforeTemplateExecutionHook>()
                    .ToList();

                templates.ForEach(x => x.BeforeTemplateExecution());

                // TODO: Run for decorators too. Probably as separate FactoryExtension
            }
        }
    }

    public interface IBeforeTemplateExecutionHook
    {
        void BeforeTemplateExecution();
    }
}