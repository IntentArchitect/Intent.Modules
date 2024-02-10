using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

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
            var cSharpFileBuilderTemplates = context.OutputTargets
                .SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<ICSharpFileBuilderTemplate>()
                .ToArray();

            var sortedSet = new SortedSet<Comparable>();
            var initial = cSharpFileBuilderTemplates
                .SelectMany(
                    collectionSelector: getActions,
                    resultSelector: (template, configuration) => new Comparable(template, configuration))
                .ToArray();

            sortedSet.UnionWith(initial);
            Debug.Assert(initial.Length == sortedSet.Count);

            while (sortedSet.Count != 0)
            {
                var action = sortedSet.First();

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
                        throw new ElementException(model.InternalElement,
                            $"An unexpected error occurred while building C# file [{template.Namespace}.{template.ClassName}] from template [{template.Id}]. See inner exception for more details:",
                            e);
                    }

                    throw new Exception(
                        $"An unexpected error occurred while building C# file [{template.Namespace}.{template.ClassName}] from template [{template.Id}]. See inner exception for more details:",
                        e);
                }
                finally
                {
                    sortedSet.Remove(action);
                }

                var toAdd = cSharpFileBuilderTemplates
                    .SelectMany(
                        collectionSelector: getActions,
                        resultSelector: (template, configuration) => new Comparable(template, configuration))
                    .ToArray();

                var sortedSetBeforeCount = sortedSet.Count;
                sortedSet.UnionWith(toAdd);
                Debug.Assert(sortedSet.Count == sortedSetBeforeCount + toAdd.Length);
            }

            foreach (var template in cSharpFileBuilderTemplates)
            {
                afterAllComplete(template);
            }
        }

        private record Comparable : IComparable<Comparable>
        {
            private static ulong _counter;

            private readonly int _order;
            private readonly ulong _insertionOrder;
            private readonly string _templateType;

            public Comparable(ICSharpFileBuilderTemplate template, (Action Invoke, int Order) configuration)
            {
                _order = configuration.Order;
                _insertionOrder = _counter++;
                _templateType = template.GetType().FullName;
                Template = template;
                Invoke = configuration.Invoke;
            }

            public ICSharpFileBuilderTemplate Template { get; }
            public Action Invoke { get; }

            public int CompareTo(Comparable other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                var orderComparison = _order.CompareTo(other._order);
                if (orderComparison != 0) return orderComparison;

                var insertionOrderComparison = _insertionOrder.CompareTo(other._insertionOrder);
                if (insertionOrderComparison != 0) return insertionOrderComparison;

                var templateTypeComparison = string.Compare(_templateType, other._templateType, StringComparison.Ordinal);
                if (templateTypeComparison != 0) return templateTypeComparison;

                var templateComparison = ReferenceEquals(Template, other.Template) ? 0 : 1;
                if (templateComparison != 0) return templateComparison;

                return ReferenceEquals(Invoke, other.Invoke) ? 0 : 1;
            }
        }
    }
}