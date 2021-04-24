using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AutoCompile.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class AutoCompileFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.ModuleBuilder.AutoCompile.AutoCompileFactoryExtension";
        public override int Order => 0;

        [IntentManaged(Mode.Ignore)]
        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterCommitChanges)
            {
                // will run after code changes has been accepted.
            }
        }
    }
}