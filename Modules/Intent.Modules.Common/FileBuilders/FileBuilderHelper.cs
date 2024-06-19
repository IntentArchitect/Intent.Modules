using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.FileBuilders;

/// <summary>
/// Helpers for <see cref="IFileBuilderTemplate"/>s.
/// </summary>
public class FileBuilderHelper
{
    /// <summary>
    /// Every time an OnBuild or AfterBuild configuration is called on a file builder class, that
    /// method could add additional OnBuild and/or AfterBuild configurations which also need to be
    /// ordered across all template instances. This system below ensures that this is happening
    /// predictably.
    /// </summary>
    public static void PerformConfiguration(ISoftwareFactoryExecutionContext context, bool isForAfterBuild)
    {
        var templates = context.OutputTargets
            .SelectMany(x => x.TemplateInstances)
            .Where(x => x.CanRunTemplate())
            .OfType<IFileBuilderTemplate>()
            .ToArray();

        var sortedSet = new SortedSet<Comparable>();
        var initial = templates
            .SelectMany(
                collectionSelector: template => !isForAfterBuild
                    ? template.File.GetConfigurationDelegates()
                    : template.File.GetConfigurationAfterDelegates(),
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
                var path = template.GetMetadata().GetRelativeFilePath();
                if (Path.DirectorySeparatorChar is '\\')
                {
                    path = path.Replace('/', '\\');
                }

                var message = $"An unexpected error occurred while building [{path}] file from template [{template.Id}]. See inner exception for more details:";

                if (template.TryGetModel<IElementWrapper>(out var model))
                {

                    throw new ElementException(model.InternalElement, message, e);
                }

                throw new Exception(message, e);
            }
            finally
            {
                sortedSet.Remove(action);
            }

            var toAdd = templates
                .SelectMany(
                    collectionSelector: template => !isForAfterBuild
                        ? template.File.GetConfigurationDelegates()
                        : template.File.GetConfigurationAfterDelegates(),
                    resultSelector: (template, configuration) => new Comparable(template, configuration))
                .ToArray();

            var sortedSetBeforeCount = sortedSet.Count;
            sortedSet.UnionWith(toAdd);
            Debug.Assert(sortedSet.Count == sortedSetBeforeCount + toAdd.Length);
        }

        foreach (var template in templates)
        {
            if (!isForAfterBuild)
            {
                template.File.MarkCompleteBuildAsDone();
            }
            else
            {
                template.File.MarkAfterBuildAsDone();
            }
        }
    }

    private record Comparable : IComparable<Comparable>
    {
        private static ulong _counter;

        private readonly int _order;
        private readonly ulong _insertionOrder;
        private readonly string _templateType;

        public Comparable(IFileBuilderTemplate template, (Action Invoke, int Order) configuration)
        {
            _order = configuration.Order;
            _insertionOrder = _counter++;
            _templateType = template.GetType().FullName;
            Template = template;
            Invoke = configuration.Invoke;
        }

        public IFileBuilderTemplate Template { get; }
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