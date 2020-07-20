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
    public abstract class ListModelTemplateRegistrationBase<TModel> : ITemplateRegistration, ISupportsConfiguration
    {
        private ParsedExpression<TModel, bool> _filter;
        private string _filterExpression;

        public abstract string TemplateId { get; }
        public abstract ITemplate CreateTemplateInstance(IProject project, IList<TModel> models);
        public abstract IList<TModel> GetModels(IApplication application);
        public string MetadataIdentifier { get; set; }

        public string FilterExpression
        {
            get
            {
                return _filterExpression;
            }
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
            var config = application.Config.GetConfig(this.TemplateId, PluginConfigType.Template);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Template : { TemplateId }.");
                return;
            }

            var models = GetModels(application);
            Logging.Log.Debug($"Models found [{typeof(TModel).FullName}] : {models.Count}");
            if (_filterExpression != null)
            {
                Logging.Log.Debug($"Applying filter : {_filterExpression}");
            }

            if (_filter != null)
            {
                models = Filter(models);
            }

            if (RegistrationAborted)
            {
                Logging.Log.Debug($"Template registration aborted : {TemplateId}");
                return;
            }
            registry.RegisterTemplate(TemplateId, project => CreateTemplateInstance((IProject) project, models));
            Logging.Log.Debug($"Template instances registered : {TemplateId}");
        }

        protected virtual bool RegistrationAborted { get; set; } = false;

        protected virtual void AbortRegistration()
        {
            RegistrationAborted = true;
        }

        public IList<TModel> Filter(IList<TModel> list)
        {
            var result = new List<TModel>();
            foreach (var model in list)
            {
                if (_filter.Invoke(model))
                {
                    result.Add(model);
                }
            }
            return result;
        }

        public virtual void Configure(IDictionary<string, string> settings)
        {
            settings.SetIfSupplied("DataFilter", (s) => FilterExpression = s);
            settings.SetIfSupplied("MetadataIdentifier", (s) => MetadataIdentifier = s);
        }
    }
}
