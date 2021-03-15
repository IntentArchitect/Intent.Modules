using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Plugins;
using Intent.Registrations;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// Use <see cref="FilePerModelTemplateRegistration{TModel}"/> instead.
    /// </summary>
    [Obsolete("Use " + nameof(FilePerModelTemplateRegistration<TModel>) + " instead.")]
    public abstract class ModelTemplateRegistrationBase<TModel> : ITemplateRegistration, ISupportsConfiguration
    {
        private ParsedExpression<TModel, bool> _filter;
        private string _filterExpression;

        public abstract string TemplateId { get; }
        public abstract ITemplate CreateTemplateInstance(IProject project, TModel model);
        public abstract IEnumerable<TModel> GetModels(IApplication application);
        public string MetadataIdentifier { get; set; }

        public string FilterExpression
        {
            get => _filterExpression;
            set
            {
                if (value == null)
                {
                    _filter = null;
                    _filterExpression = null;
                }
                else
                {
                    if (value != _filterExpression)
                    {
                        _filter = ExpressionParser.Parse<TModel, bool>(value, "model");
                    }
                    _filterExpression = value;
                }
            }
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var config = application.Config.GetConfig(TemplateId, PluginConfigType.Template);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Template : { TemplateId }.");
                return;
            }

            var templateInstancesRegistered = 0;
            var models = GetModels(application).ToArray();
            Logging.Log.Debug($"Models found [{typeof(TModel).FullName}] : {models.Length}");
            if (_filterExpression != null)
            {
                Logging.Log.Debug($"Applying filter : {_filterExpression}");
            }

            foreach (var model in models)
            {
                if (_filter == null || _filter.Invoke(model))
                {
                    registry.RegisterTemplate(TemplateId, project => CreateTemplateInstance((IProject) project, model));
                    templateInstancesRegistered++;
                }
            }
            Logging.Log.Debug($"Template instances registered : {templateInstancesRegistered}");
        }

        public virtual void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied("DataFilter", (s) => FilterExpression = s);
            settings.SetIfSupplied("MetadataIdentifier", (s) => MetadataIdentifier = s);
        }
    }
}
