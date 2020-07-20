using Intent.Configuration;
using Intent.Engine;
using Intent.Registrations;
using Intent.Utils;

namespace Intent.Modules.Common.Registrations
{
    public abstract class TemplateRegistrationBase : ITemplateRegistration
    {
        public abstract string TemplateId { get; }
        
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var config = application.Config.GetConfig(this.TemplateId, PluginConfigType.Template);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Template : { TemplateId }.");
                return;
            }
            if (RegistrationAborted)
            {
                Logging.Log.Debug($"Template registration aborted : {TemplateId}");
                return;
            }

            Register(registry, application);
        }

        protected abstract void Register(ITemplateInstanceRegistry registry, IApplication application);

        protected virtual bool RegistrationAborted { get; set; } = false;

        protected virtual void AbortRegistration()
        {
            RegistrationAborted = true;
        }
    }
}