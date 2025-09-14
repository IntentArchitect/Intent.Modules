using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Templates;

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

        var initialItems = templates
            .SelectMany(
                collectionSelector: template => !isForAfterBuild
                    ? template.File.GetConfigurationDelegates()
                    : template.File.GetConfigurationAfterDelegates(),
                resultSelector: (template, action) => new Item(template, action));

        var queue = new PriorityQueue<Item, Item>(initialItems.Select(x => (x, x)));

        while (queue.TryDequeue(out var item, out _))
        {
            try
            {
                item.Action();
            }
            catch (ElementException)
            {
                throw;
            }
            catch (Exception e)
            {
                var template = item.Template;
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

            var toAdd = templates
                .SelectMany(
                    collectionSelector: template => !isForAfterBuild
                        ? template.File.GetConfigurationDelegates()
                        : template.File.GetConfigurationAfterDelegates(),
                    resultSelector: (template, configuration) => new Item(template, configuration))
                .ToArray();

            foreach (var comparable in toAdd)
            {
                queue.Enqueue(comparable, comparable);
            }
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

    private record Item : IComparable<Item>
    {
        private static ulong _counter;

        private readonly int _order;
        private readonly ulong _creationOrder;
        private readonly string _templateType;
        private readonly string _modelId;

        public Item(IFileBuilderTemplate template, (Action Invoke, int Order) configuration)
        {
            _order = configuration.Order;
            _creationOrder = _counter++;
            _templateType = template.GetType().FullName;
            _modelId = ((template as ITemplateWithModel)?.Model as IMetadataModel)?.Id;
            Template = template;
            Action = configuration.Invoke;
        }

        public IFileBuilderTemplate Template { get; }
        public Action Action { get; }

        public int CompareTo(Item other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var orderComparison = _order.CompareTo(other._order);
            if (orderComparison != 0) return orderComparison;

            var templateTypeComparison = string.Compare(_templateType, other._templateType, StringComparison.Ordinal);
            if (templateTypeComparison != 0) return templateTypeComparison;

            var templateIdComparison = string.Compare(Template.Id, other.Template.Id, StringComparison.Ordinal);
            if (templateIdComparison != 0) return templateIdComparison;

            var modelIdComparison = string.Compare(_modelId, other._modelId, StringComparison.Ordinal);
            if (modelIdComparison != 0) return modelIdComparison;

            var creationOrderComparison = _creationOrder.CompareTo(other._creationOrder);
            if (creationOrderComparison != 0) return creationOrderComparison;

            return ReferenceEquals(Action, other.Action) ? 0 : 1;
        }

        //public override int GetHashCode()
        //{
        //    return Invoke.GetHashCode();
        //}

        //public override bool Equals(object obj)
        //{
        //    return ReferenceEquals(this, obj);
        //}
    }
}