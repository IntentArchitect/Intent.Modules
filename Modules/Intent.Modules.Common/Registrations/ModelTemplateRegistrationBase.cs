using System.Collections.Generic;
using System.Linq;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Helpers;
using Intent.SoftwareFactory.Registrations;

namespace Intent.SoftwareFactory.Templates.Registrations
{
    public abstract class ModelTemplateRegistrationBase<TModel> : IProjectTemplateRegistration, ISupportsConfiguration
    {
        private ParsedExpression<TModel, bool> _filter;
        private string _filterExpression;

        public abstract string TemplateId { get; }
        public abstract ITemplate CreateTemplateInstance(IProject project, TModel model);
        public abstract IEnumerable<TModel> GetModels(IApplication applicationManager);
        public string MetaDataIdentifier { get; set; }

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
                    registry.Register(TemplateId, project => CreateTemplateInstance(project, model));
                    templateInstancesRegistered++;
                }
            }
            Logging.Log.Debug($"Template instances registered : {templateInstancesRegistered}");
        }

        public virtual void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied("DataFilter", (s) => FilterExpression = s);
            settings.SetIfSupplied("MetaDataIdentifier", (s) => MetaDataIdentifier = s);
        }
    }
}
