#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <inheritdoc />
    public class TemplateDependency : ITemplateDependency
    {
        private readonly object? _context;
        private readonly Func<ITemplate, bool> _isMatch;

        private TemplateDependency(string templateId, Func<ITemplate, bool> isMatch, object? context, IOutputTarget? accessibleTo)
        {
            TemplateId = templateId;
            AccessibleTo = accessibleTo;
            _context = context;
            _isMatch = isMatch;
        }

        /// <inheritdoc />
        public string TemplateId { get; }

        /// <inheritdoc />
        public IOutputTarget? AccessibleTo { get; }

        /// <summary>
        /// This will be changed to a private member, please contact support@intentarchitect.com if you have a need for this.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public object? Context => _context;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Template Id: {TemplateId}{(_context != null ? " - " + _context : string.Empty)}";
        }

        /// <summary>
        /// Overload of <see cref="OfType{T}(IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OfType<TTemplate>() =>
            OfType<TTemplate>(accessibleTo: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which are of the
        /// provided <typeparamref name="TTemplate"/> type.
        /// </summary>
        /// <typeparam name="TTemplate"></typeparam>
        /// <param name="accessibleTo">Optional. The resolved template must be accessible to the provided <see cref="IOutputTarget"/>.</param>
        public static ITemplateDependency OfType<TTemplate>(IOutputTarget? accessibleTo)
        {
            return OfTypeTemplateDependency<TTemplate>.Create(accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnTemplate(string,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnTemplate(string templateIdOrRole) =>
            OnTemplate(templateIdOrRole, accessibleTo: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrRole"/>.
        /// </summary>
        /// <param name="templateIdOrRole"></param>
        /// <param name="accessibleTo">Optional. The resolved template must be accessible to the provided <see cref="IOutputTarget"/>.</param>
        public static ITemplateDependency OnTemplate(string templateIdOrRole, IOutputTarget? accessibleTo)
        {
            return TemplateIdTemplateDependency.Create(templateIdOrRole, accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnTemplate(ITemplate,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnTemplate(ITemplate template) =>
            OnTemplate(template, accessibleTo: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds a template whose reference
        /// matches the provided <see cref="ITemplate"/>.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="accessibleTo">Optional. The resolved template must be accessible to the provided <see cref="IOutputTarget"/>.</param>
        public static ITemplateDependency OnTemplate(ITemplate template, IOutputTarget? accessibleTo)
        {
            return TemplateInstanceTemplateDependency.Create(template, accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnModel(string,object,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel) =>
            OnModel(templateIdOrName, metadataModel, accessibleTo: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a <see cref="ITemplateWithModel"/>
        /// whose <see cref="ITemplateWithModel.Model"/>'s reference matches that of the provided
        /// <paramref name="metadataModel"/>.
        /// </summary>
        /// <param name="templateIdOrName"></param>
        /// <param name="metadataModel"></param>
        /// <param name="accessibleTo">Optional. The resolved template must be accessible to the provided <see cref="IOutputTarget"/>.</param>
        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel, IOutputTarget? accessibleTo)
        {
            return ModelTemplateDependency.Create(templateIdOrName, metadataModel, accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnModel(string,IMetadataModel,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel) =>
            OnModel(templateIdOrName, metadataModel, accessibleTo: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a
        /// <see cref="ITemplateWithModel"/> whose <see cref="ITemplateWithModel.Model"/> is a
        /// <see cref="IMetadataModel"/> with its <see cref="IMetadataModel.Id"/> matching that
        /// the provided <paramref name="metadataModel"/>.
        /// </summary>
        /// <param name="metadataModel"></param>
        /// <param name="accessibleTo">Optional. The resolved template must be accessible to the provided <see cref="IOutputTarget"/>.</param>
        /// <param name="templateIdOrName"></param>
        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel, IOutputTarget? accessibleTo)
        {
            return ModelIdTemplateDependency.Create(templateIdOrName, metadataModel.Id, accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnModel{T}(string,Func{T,bool},IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch) =>
            OnModel(templateIdOrName, isMatch, accessibleTo: null);

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, IOutputTarget? accessibleTo)
        {
            return OnModel(templateIdOrName, isMatch, null, accessibleTo);
        }

        /// <summary>
        /// Overload of <see cref="OnModel{T}(string,Func{T,bool},object,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object context) =>
            OnModel(templateIdOrName, isMatch, context, accessibleTo: null);

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object? context, IOutputTarget? accessibleTo)
        {
            return new TemplateDependency(
                templateId: templateIdOrName,
                isMatch: template =>
                    template is ITemplateWithModel { Model: TModel tModel } &&
                    isMatch(tModel),
                context: context,
                accessibleTo: accessibleTo);
        }

        /// <inheritdoc />
        public bool IsMatch(ITemplate template)
        {
            return _isMatch(template);
        }

        private class TemplateInstanceTemplateDependency : FastLookupTemplateDependency<(ITemplate Template, IOutputTarget? AccessibleTo), TemplateInstanceTemplateDependency>
        {
            private readonly ITemplate _template;

            private TemplateInstanceTemplateDependency(ITemplate template, IOutputTarget? accessibleTo) : base(accessibleTo)
            {
                _template = template;
            }

            public static ITemplateDependency Create(ITemplate template, IOutputTarget? accessibleTo)
            {
                if (template == null) throw new ArgumentNullException(nameof(template));

                return InstanceCache.GetOrAdd((template, accessibleTo), _ => new TemplateInstanceTemplateDependency(template, accessibleTo));
            }

            public override bool TryGetWithAccessibleTo(IOutputTarget? accessibleTo, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
            {
                if (AccessibleTo != null || accessibleTo == null)
                {
                    templateDependency = null;
                    return false;
                }

                templateDependency = Create(_template, accessibleTo);
                return true;
            }

            protected override ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return _template;
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                yield return _template;
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(_template);
            }

            public override string TemplateId => _template.Id;

            public override bool IsMatch(ITemplate template)
            {
                return ReferenceEquals(_template, template);
            }
        }

        private class TemplateIdTemplateDependency : FastLookupTemplateDependency<(string TemplateId, IOutputTarget? AccessibleTo), TemplateIdTemplateDependency>
        {
            private TemplateIdTemplateDependency(string templateId, IOutputTarget? accessibleTo) : base(accessibleTo)
            {
                TemplateId = templateId ?? throw new ArgumentNullException(nameof(templateId));
            }

            public static ITemplateDependency Create(string templateId, IOutputTarget? accessibleTo)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));

                return InstanceCache.GetOrAdd((templateId, accessibleTo), _ => new TemplateIdTemplateDependency(templateId, accessibleTo));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, AccessibleTo, AlwaysReturnTrue);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, AccessibleTo, AlwaysReturnTrue);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, AccessibleTo);
            }

            private static bool AlwaysReturnTrue(ITemplate _) => true;

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template.Id == TemplateId;
            }
        }

        private class ModelTemplateDependency : FastLookupTemplateDependency<(string TemplateId, object Model, IOutputTarget? AccessibleTo), ModelTemplateDependency>
        {
            private readonly object _model;

            private ModelTemplateDependency(string templateId, object model, IOutputTarget? accessibleTo) : base(accessibleTo)
            {
                TemplateId = templateId;
                _model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public static ITemplateDependency Create(string templateId, object model, IOutputTarget? accessibleTo)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (model == null) throw new ArgumentNullException(nameof(model));

                return InstanceCache.GetOrAdd((templateId, model, accessibleTo), _ => new ModelTemplateDependency(templateId, model, accessibleTo));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _model, AccessibleTo);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _model, AccessibleTo);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _model, AccessibleTo);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel templateWithModel &&
                       ReferenceEquals(_model, templateWithModel.Model);
            }
        }

        private class ModelIdTemplateDependency : FastLookupTemplateDependency<(string TemplateId, string ModelId, IOutputTarget? AccessibleTo), ModelIdTemplateDependency>
        {
            private readonly string _modelId;

            private ModelIdTemplateDependency(string templateId, string modelId, IOutputTarget? accessibleTo) : base(accessibleTo)
            {
                TemplateId = templateId;
                _modelId = modelId;
            }

            public static ITemplateDependency Create(string templateId, string modelId, IOutputTarget? accessibleTo)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (modelId == null) throw new ArgumentNullException(nameof(modelId));

                return InstanceCache.GetOrAdd((templateId, modelId, accessibleTo), _ => new ModelIdTemplateDependency(templateId, modelId, accessibleTo));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _modelId, AccessibleTo);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _modelId, AccessibleTo);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _modelId, AccessibleTo);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel { Model: IMetadataModel metadataModel } &&
                       metadataModel.Id == _modelId;
            }
        }

        private class OfTypeTemplateDependency<TTemplate> : FastLookupTemplateDependency<(Type Type, IOutputTarget? AccessibleTo), OfTypeTemplateDependency<TTemplate>>
        {
            private readonly TemplateDependency _templateDependency = new();

            private OfTypeTemplateDependency(IOutputTarget? accessibleTo) : base(accessibleTo) { }

            public static OfTypeTemplateDependency<TTemplate> Create(IOutputTarget? accessibleTo)
            {
                return InstanceCache.GetOrAdd((typeof(TTemplate), accessibleTo), _ => new OfTypeTemplateDependency<TTemplate>(accessibleTo));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(_templateDependency);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances<ITemplate>(_templateDependency);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(_templateDependency);
            }

            public override string? TemplateId => null;

            public override bool IsMatch(ITemplate template)
            {
                throw new InvalidOperationException();
            }

            private class TemplateDependency : ITemplateDependency
            {
                public string? TemplateId => null;

                public bool IsMatch(ITemplate template)
                {
                    return template is TTemplate;
                }
            }
        }

        private abstract class FastLookupTemplateDependency<TInstanceCacheKey, TTemplateDependency> : IFastLookupTemplateDependency
            where TTemplateDependency : ITemplateDependency where TInstanceCacheKey : notnull
        {
            protected FastLookupTemplateDependency(IOutputTarget? accessibleTo)
            {
                AccessibleTo = accessibleTo;
            }

            /// <summary>
            /// Avoids additional memory allocations and also improves effectiveness of <see cref="_cachedLookupTemplateInstance"/>.
            /// </summary>
            protected static readonly ConcurrentDictionary<TInstanceCacheKey, TTemplateDependency> InstanceCache = new();

            private ITemplate? _cachedLookupTemplateInstance;
            private IEnumerable<ITemplate>? _cachedLookupTemplateInstances;
            private IOutputTarget? _cachedLookupOutputTarget;

            ITemplate? IFastLookupTemplateDependency.LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupTemplateInstance ??= LookupTemplateInstance(context);
            }

            IEnumerable<ITemplate> IFastLookupTemplateDependency.LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupTemplateInstances ??= LookupTemplateInstances(context);
            }

            IOutputTarget? IFastLookupTemplateDependency.LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupOutputTarget ??= LookupOutputTarget(context);
            }

            public virtual bool TryGetWithAccessibleTo(IOutputTarget? accessibleTo, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
            {
                templateDependency = null;
                return false;
            }

            protected abstract ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context);

            protected abstract IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context);

            protected abstract IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context);

            public abstract string? TemplateId { get; }

            public abstract bool IsMatch(ITemplate template);

            public IOutputTarget? AccessibleTo { get; }
        }
    }
}
