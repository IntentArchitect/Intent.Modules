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

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.Projects.SelectMany(x => x.TemplateInstances)
                .OfType<IAfterTemplateRegistrationExecutionHook>()
                .ToList();

            templates.ForEach(x => x.AfterTemplateRegistration());
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.Projects.SelectMany(x => x.TemplateInstances)
                .OfType<ITemplateBeforeExecutionHook>()
                .ToList();

            templates.ForEach(x => x.BeforeTemplateExecution());
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