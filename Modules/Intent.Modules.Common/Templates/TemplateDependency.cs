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

        private TemplateDependency(string templateId, Func<ITemplate, bool> isMatch, object? context, IOutputTarget? source)
        {
            TemplateId = templateId;
            Source = source;
            _context = context;
            _isMatch = isMatch;
        }

        /// <inheritdoc />
        public string TemplateId { get; }

        /// <inheritdoc />
        public IOutputTarget? Source { get; }

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
            OfType<TTemplate>(source: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which are of the
        /// provided <typeparamref name="TTemplate"/> type.
        /// </summary>
        public static ITemplateDependency OfType<TTemplate>(IOutputTarget? source)
        {
            return OfTypeTemplateDependency<TTemplate>.Create(source);
        }

        /// <summary>
        /// Overload of <see cref="OnTemplate(string,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnTemplate(string templateIdOrRole) =>
            OnTemplate(templateIdOrRole, source: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrRole"/>.
        /// </summary>
        public static ITemplateDependency OnTemplate(string templateIdOrRole, IOutputTarget? source)
        {
            return TemplateIdTemplateDependency.Create(templateIdOrRole, source);
        }

        /// <summary>
        /// Overload of <see cref="OnTemplate(ITemplate,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnTemplate(ITemplate template) =>
            OnTemplate(template, source: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds a template whose reference
        /// matches the provided <see cref="ITemplate"/>.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="source">Optional. The source template which requiring the dependency</param>
        public static ITemplateDependency OnTemplate(ITemplate template, IOutputTarget? source)
        {
            return TemplateInstanceTemplateDependency.Create(template, source);
        }

        /// <summary>
        /// Overload of <see cref="OnModel(string,object,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel) =>
            OnModel(templateIdOrName, metadataModel, source: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a <see cref="ITemplateWithModel"/>
        /// whose <see cref="ITemplateWithModel.Model"/>'s reference matches that of the provided
        /// <paramref name="metadataModel"/>.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel, IOutputTarget? source)
        {
            return ModelTemplateDependency.Create(templateIdOrName, metadataModel, source);
        }

        /// <summary>
        /// Overload of <see cref="OnModel(string,IMetadataModel,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel) =>
            OnModel(templateIdOrName, metadataModel, source: null);

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a
        /// <see cref="ITemplateWithModel"/> whose <see cref="ITemplateWithModel.Model"/> is a
        /// <see cref="IMetadataModel"/> with its <see cref="IMetadataModel.Id"/> matching that
        /// the provided <paramref name="metadataModel"/>.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel, IOutputTarget? source)
        {
            return ModelIdTemplateDependency.Create(templateIdOrName, metadataModel.Id, source);
        }

        /// <summary>
        /// Overload of <see cref="OnModel{T}(string,Func{T,bool},IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch) =>
            OnModel(templateIdOrName, isMatch, source: null);

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, IOutputTarget? source)
        {
            return OnModel(templateIdOrName, isMatch, null, source);
        }

        /// <summary>
        /// Overload of <see cref="OnModel{T}(string,Func{T,bool},object,IOutputTarget)"/> to maintain binary
        /// backwards compatibility.
        /// </summary>
        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object context) =>
            OnModel(templateIdOrName, isMatch, context, source: null);

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object? context, IOutputTarget? source)
        {
            return new TemplateDependency(
                templateId: templateIdOrName,
                isMatch: template =>
                    template is ITemplateWithModel { Model: TModel tModel } &&
                    isMatch(tModel),
                context: context,
                source: source);
        }

        /// <inheritdoc />
        public bool IsMatch(ITemplate template)
        {
            return _isMatch(template);
        }

        private class TemplateInstanceTemplateDependency : FastLookupTemplateDependency<(ITemplate, IOutputTarget?), TemplateInstanceTemplateDependency>
        {
            private readonly ITemplate _template;

            private TemplateInstanceTemplateDependency(ITemplate template, IOutputTarget? source) : base(source)
            {
                _template = template;
            }

            public static ITemplateDependency Create(ITemplate template, IOutputTarget? source)
            {
                if (template == null) throw new ArgumentNullException(nameof(template));

                return InstanceCache.GetOrAdd((template, source), _ => new TemplateInstanceTemplateDependency(template, source));
            }

            public override bool TryGetWithSource(IOutputTarget? source, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
            {
                if (Source != null || source == null)
                {
                    templateDependency = null;
                    return false;
                }

                templateDependency = Create(_template, source);
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

        private class TemplateIdTemplateDependency : FastLookupTemplateDependency<(string, IOutputTarget?), TemplateIdTemplateDependency>
        {
            private TemplateIdTemplateDependency(string templateId, IOutputTarget? source) : base(source)
            {
                TemplateId = templateId ?? throw new ArgumentNullException(nameof(templateId));
            }

            public static ITemplateDependency Create(string templateId, IOutputTarget? source)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));

                return InstanceCache.GetOrAdd((templateId, source), _ => new TemplateIdTemplateDependency(templateId, source));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, Source, AlwaysReturnTrue);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, Source, AlwaysReturnTrue);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, Source);
            }

            private static bool AlwaysReturnTrue(ITemplate _) => true;

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template.Id == TemplateId;
            }
        }

        private class ModelTemplateDependency : FastLookupTemplateDependency<(string, object, IOutputTarget?), ModelTemplateDependency>
        {
            private readonly object _model;

            private ModelTemplateDependency(string templateId, object model, IOutputTarget? source) : base(source)
            {
                TemplateId = templateId;
                _model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public static ITemplateDependency Create(string templateId, object model, IOutputTarget? source)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (model == null) throw new ArgumentNullException(nameof(model));

                return InstanceCache.GetOrAdd((templateId, model, source), _ => new ModelTemplateDependency(templateId, model, source));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _model, Source);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _model, Source);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _model, Source);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel templateWithModel &&
                       ReferenceEquals(_model, templateWithModel.Model);
            }
        }

        private class ModelIdTemplateDependency : FastLookupTemplateDependency<(string, string, IOutputTarget?), ModelIdTemplateDependency>
        {
            private readonly string _modelId;

            private ModelIdTemplateDependency(string templateId, string modelId, IOutputTarget? source) : base(source)
            {
                TemplateId = templateId;
                _modelId = modelId;
            }

            public static ITemplateDependency Create(string templateId, string modelId, IOutputTarget? source)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (modelId == null) throw new ArgumentNullException(nameof(modelId));

                return InstanceCache.GetOrAdd((templateId, modelId, source), _ => new ModelIdTemplateDependency(templateId, modelId, source));
            }

            protected override ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _modelId, Source);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _modelId, Source);
            }

            protected override IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _modelId, Source);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel { Model: IMetadataModel metadataModel } &&
                       metadataModel.Id == _modelId;
            }
        }

        private class OfTypeTemplateDependency<TTemplate> : FastLookupTemplateDependency<(Type, IOutputTarget?), OfTypeTemplateDependency<TTemplate>>
        {
            private readonly TemplateDependency _templateDependency = new();

            private OfTypeTemplateDependency(IOutputTarget? source) : base(source) { }

            public static OfTypeTemplateDependency<TTemplate> Create(IOutputTarget? source)
            {
                return InstanceCache.GetOrAdd((typeof(TTemplate), source), _ => new OfTypeTemplateDependency<TTemplate>(source));
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
            protected FastLookupTemplateDependency(IOutputTarget? source)
            {
                Source = source;
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

            public virtual bool TryGetWithSource(IOutputTarget? source, [NotNullWhen(true)] out ITemplateDependency? templateDependency)
            {
                templateDependency = null;
                return false;
            }

            protected abstract ITemplate? LookupTemplateInstance(ISoftwareFactoryExecutionContext context);

            protected abstract IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context);

            protected abstract IOutputTarget? LookupOutputTarget(ISoftwareFactoryExecutionContext context);

            public abstract string? TemplateId { get; }

            public abstract bool IsMatch(ITemplate template);

            public IOutputTarget? Source { get; }
        }
    }
}
