using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Helpers;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Registrations
{
    public abstract class ListModelTemplateRegistrationBase<TModel> : IProjectTemplateRegistration, ISupportsConfiguration
    {
        private ParsedExpression<TModel, bool> _filter;
        private string _filterExpression;

        public abstract string TemplateId { get; }
        public abstract ITemplate CreateTemplateInstance(IProject project, IList<TModel> models);
        public abstract IList<TModel> GetModels(IApplication applicationManager);

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

        public void DoRegistration(Action<string, Func<IProject, ITemplate>> register, IApplication application)
        {

            var config = application.Config.GetConfig(this.TemplateId, Configuration.PluginConfigType.Template);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Template : { TemplateId }.");
                return;
            }            

            int templateInstancesRegistered = 0;
            var models = GetModels(application);
            Logging.Log.Debug($"Models found : {models.Count()}");
            if (_filterExpression != null)
            {
                Logging.Log.Debug($"Applying filter : {_filterExpression}");
            }

            if (_filter != null)
            {
                models = Filter(models);
            }

            register(TemplateId, project => CreateTemplateInstance(project, models));
            templateInstancesRegistered++;
            Logging.Log.Debug($"Template instances registered : {templateInstancesRegistered}");
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
        }
    }
}
