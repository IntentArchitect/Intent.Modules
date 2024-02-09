using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Templates;

namespace Intent.Modules.Common.Plugins
{
    [Description("Intent.Common.TemplateLifeCycleHooks")]
    public class CSharpFileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.CSharp.CSharpFileBuilderFactoryExtension";

        public override int Order => 100_000_000; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            PerformConfiguration(
                context: application,
                getActions: t => t.CSharpFile.GetConfigurationDelegates(),
                afterAllComplete: null);

            PerformConfiguration(
                context: application,
                getActions: t => t.CSharpFile.GetConfigurationDelegates(),
                afterAllComplete: t => t.CSharpFile.MarkCompleteBuildAsDone());
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            PerformConfiguration(
                context: application,
                getActions: t => t.CSharpFile.GetConfigurationAfterDelegates(),
                afterAllComplete: t => t.CSharpFile.MarkAfterBuildAsDone());
        }

        private static void PerformConfiguration(
            ISoftwareFactoryExecutionContext context,
            Func<ICSharpFileBuilderTemplate, IReadOnlyCollection<(Action Invoke, int Order)>> getActions,
            Action<ICSharpFileBuilderTemplate> afterAllComplete)
        {
            var cSharpFileBuilderTemplates = context.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            while (true)
            {
                var actions = cSharpFileBuilderTemplates
                    .SelectMany(
                        collectionSelector: getActions,
                        resultSelector: (template, configuration) => new
                        {
                            Template = template,
                            TemplateType = template.GetType().ToString(),
                            configuration.Order,
                            configuration.Invoke
                        })
                    .OrderBy(x => x.Order)
                    .ThenBy(x => x.TemplateType)
                    .ToArray();

                if (actions.Length == 0)
                {
                    break;
                }

                foreach (var action in actions)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (ElementException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        var template = action.Template;

                        if (template.TryGetModel<IElementWrapper>(out var model))
                        {
                            throw new ElementException(model.InternalElement, $"An unexpected error occurred while building C# file [{template.Namespace}.{template.ClassName}] from template [{template.Id}]. See inner exception for more details:", e);
                        }

                        throw new Exception($"An unexpected error occurred while building C# file [{template.Namespace}.{template.ClassName}] from template [{template.Id}]. See inner exception for more details:", e);
                    }
                }
            }

            if (afterAllComplete == null)
            {
                return;
            }

            foreach (var template in cSharpFileBuilderTemplates)
            {
                afterAllComplete(template);
            }
        }
    }
}