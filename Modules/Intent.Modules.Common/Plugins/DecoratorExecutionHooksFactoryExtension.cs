using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intent.Modules.Common.Plugins
{
    public class DecoratorExecutionHooksFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        public override string Id => "Intent.Common.DecoratorExecutionHooks";
        public override int Order => -1;

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateExecution)
            {
                // This will only return the method name. This is done in order to make reflection
                // names part of static analysis instead of just being a plain string that will only
                // reveal at runtime if there are problems
                var getDecoratorsName = nameof(IHasDecorators<DummyDecorator>.GetDecorators);

                application.Projects
                    .SelectMany(x => x.TemplateInstances)
                    .Where(p => p
                        .GetType()
                        .GetInterfaces()
                        .Any(q => q.IsGenericType && q.GetGenericTypeDefinition() == typeof(IHasDecorators<>)))
                    .SelectMany(s => s.GetType()
                        .GetMethod(getDecoratorsName, BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(s, null) as IEnumerable<object>)
                    .OfType<IDecoratorExecutionHooks>()
                    .ToList()
                    .ForEach(x => x.BeforeTemplateExecution());
            }
        }

        // This class solely exists for the "nameof" statement to work since the interface needs a generic parameter
        private class DummyDecorator : ITemplateDecorator
        {
            public int Priority => throw new System.NotImplementedException();
        }
    }
}
