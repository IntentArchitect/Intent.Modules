using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    /// <inheritdoc />
    public class TemplateDependency : ITemplateDependency
    {
        private readonly object _context;
        private readonly Func<ITemplate, bool> _isMatch;

        private TemplateDependency(string templateId, Func<ITemplate, bool> isMatch, object context)
        {
            TemplateId = templateId;
            _context = context;
            _isMatch = isMatch;
        }

        /// <inheritdoc />
        public string TemplateId { get; }

        /// <summary>
        /// This will be changed to a private member, please contact support@intentarchitect.com if you have a need for this.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version_3_2_0)]
        public object Context => _context;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Template Id: {TemplateId}{(_context != null ? " - " + _context : string.Empty)}";
        }

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/>.
        /// </summary>
        public static ITemplateDependency OnTemplate(string templateIdOrName)
        {
            return TemplateIdTemplateDependency.Create(templateIdOrName);
        }

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds a template whose reference
        /// matches the provided <see cref="ITemplate"/>.
        /// </summary>
        public static ITemplateDependency OnTemplate(ITemplate template)
        {
            return TemplateInstanceTemplateDependency.Create(template);
        }

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a <see cref="ITemplateWithModel"/>
        /// whose <see cref="ITemplateWithModel.Model"/>'s reference matches that of the provided
        /// <paramref name="metadataModel"/>.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, object metadataModel)
        {
            return ModelTemplateDependency.Create(templateIdOrName, metadataModel);
        }

        /// <summary>
        /// Returns a <see cref="ITemplateDependency"/> which finds templates which have the
        /// provided <paramref name="templateIdOrName"/> and which are a
        /// <see cref="ITemplateWithModel"/> whose <see cref="ITemplateWithModel.Model"/> is a
        /// <see cref="IMetadataModel"/> with its <see cref="IMetadataModel.Id"/> matching that
        /// the provided <paramref name="metadataModel"/>.
        /// </summary>
        public static ITemplateDependency OnModel(string templateIdOrName, IMetadataModel metadataModel)
        {
            return ModelIdTemplateDependency.Create(templateIdOrName, metadataModel.Id);
        }

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch)
        {
            return TemplateDependency.OnModel(templateIdOrName, isMatch, null);
        }

        public static ITemplateDependency OnModel<TModel>(string templateIdOrName, Func<TModel, bool> isMatch, object context)
        {
            return new TemplateDependency(
                templateId: templateIdOrName,
                isMatch: template =>
                    template is ITemplateWithModel templateWithModel &&
                    templateWithModel.Model is TModel tModel &&
                    isMatch(tModel),
                context: context);
        }

        /// <inheritdoc />
        public bool IsMatch(ITemplate template)
        {
            return _isMatch(template);
        }

        private class TemplateInstanceTemplateDependency : FastLookupTemplateDependency<ITemplate, TemplateInstanceTemplateDependency>
        {
            private readonly ITemplate _template;

            private TemplateInstanceTemplateDependency(ITemplate template)
            {
                _template = template;
            }

            public static ITemplateDependency Create(ITemplate template)
            {
                if (template == null) throw new ArgumentNullException(nameof(template));

                return InstanceCache.GetOrAdd(template, key => new TemplateInstanceTemplateDependency(key));
            }

            protected override ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return _template;
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                yield return _template;
            }

            protected override IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(_template);
            }

            public override string TemplateId => _template.Id;

            public override bool IsMatch(ITemplate template)
            {
                return ReferenceEquals(_template, template);
            }
        }

        private class TemplateIdTemplateDependency : FastLookupTemplateDependency<string, TemplateIdTemplateDependency>
        {
            private TemplateIdTemplateDependency(string templateId)
            {
                TemplateId = templateId ?? throw new ArgumentNullException(nameof(templateId));
            }

            public static ITemplateDependency Create(string templateId)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));

                return InstanceCache.GetOrAdd(templateId, key => new TemplateIdTemplateDependency(key));
            }

            protected override ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, AlwaysReturnTrue);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, AlwaysReturnTrue);
            }

            protected override IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId);
            }

            private static bool AlwaysReturnTrue(ITemplate _) => true;

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template.Id == TemplateId;
            }
        }

        private class ModelTemplateDependency : FastLookupTemplateDependency<(string, object), ModelTemplateDependency>
        {
            private readonly object _model;

            private ModelTemplateDependency(string templateId, object model)
            {
                TemplateId = templateId;
                _model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public static ITemplateDependency Create(string templateId, object model)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (model == null) throw new ArgumentNullException(nameof(model));

                return InstanceCache.GetOrAdd((templateId, model), _ => new ModelTemplateDependency(templateId, model));
            }

            protected override ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _model);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _model);
            }

            protected override IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _model);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel templateWithModel &&
                       ReferenceEquals(_model, templateWithModel.Model);
            }
        }

        private class ModelIdTemplateDependency : FastLookupTemplateDependency<(string, string), ModelIdTemplateDependency>
        {
            private readonly string _modelId;

            private ModelIdTemplateDependency(string templateId, string modelId)
            {
                TemplateId = templateId;
                _modelId = modelId;
            }

            public static ITemplateDependency Create(string templateId, string modelId)
            {
                if (templateId == null) throw new ArgumentNullException(nameof(templateId));
                if (modelId == null) throw new ArgumentNullException(nameof(modelId));

                return InstanceCache.GetOrAdd((templateId, modelId), _ => new ModelIdTemplateDependency(templateId, modelId));
            }

            protected override ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstance(TemplateId, _modelId);
            }

            protected override IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return context.FindTemplateInstances(TemplateId, _modelId);
            }

            protected override IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return context.FindOutputTargetWithTemplate(TemplateId, _modelId);
            }

            public override string TemplateId { get; }

            public override bool IsMatch(ITemplate template)
            {
                return template is ITemplateWithModel templateWithModel &&
                       templateWithModel.Model is IMetadataModel metadataModel &&
                       metadataModel.Id == _modelId;
            }
        }

        private abstract class FastLookupTemplateDependency<TInstanceCacheKey, TTemplateDependency> : IFastLookupTemplateDependency
            where TTemplateDependency : ITemplateDependency
        {
            /// <summary>
            /// Avoids additional memory allocations and also improves effectiveness of <see cref="_cachedLookupTemplateInstance"/>.
            /// </summary>
            protected static readonly ConcurrentDictionary<TInstanceCacheKey, TTemplateDependency> InstanceCache = new ConcurrentDictionary<TInstanceCacheKey, TTemplateDependency>();

            private ITemplate _cachedLookupTemplateInstance;
            private IEnumerable<ITemplate> _cachedLookupTemplateInstances;
            private IOutputTarget _cachedLookupOutputTarget;

            ITemplate IFastLookupTemplateDependency.LookupTemplateInstance(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupTemplateInstance ?? (_cachedLookupTemplateInstance = LookupTemplateInstance(context));
            }

            IEnumerable<ITemplate> IFastLookupTemplateDependency.LookupTemplateInstances(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupTemplateInstances ?? (_cachedLookupTemplateInstances = LookupTemplateInstances(context));
            }

            IOutputTarget IFastLookupTemplateDependency.LookupOutputTarget(ISoftwareFactoryExecutionContext context)
            {
                return _cachedLookupOutputTarget ?? (_cachedLookupOutputTarget = LookupOutputTarget(context));
            }

            protected abstract ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context);

            protected abstract IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context);

            protected abstract IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context);

            public abstract string TemplateId { get; }

            public abstract bool IsMatch(ITemplate template);
        }
    }
}
