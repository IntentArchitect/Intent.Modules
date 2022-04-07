using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Registrations;
using Intent.Utils;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// A generic base implementation of <seealso cref="ITemplateRegistration"/>.
    /// <para>
    /// The <see cref="Register"/> method will be called once by the Software Factory Execution.
    /// </para>
    /// </summary>
    public abstract class TemplateRegistrationBase : ITemplateRegistration
    {
        private bool _registrationAborted = false;
        private bool _doRegistrationCalled = false;

        public abstract string TemplateId { get; }

        public virtual void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            _doRegistrationCalled = true;
            var config = application.Config.GetConfig(this.TemplateId, PluginConfigType.Template);
            if (!config.Enabled)
            {
                Logging.Log.Info($"Skipping disabled Template : { TemplateId }.");
                return;
            }
            if (_registrationAborted)
            {
                Logging.Log.Debug($"Template registration aborted : {TemplateId}");
                return;
            }

            Register(registry, application);
        }

        /// <summary>
        /// This method is called once by the Software Factory Execution.
        /// <para>
        /// Registration of template instances can be done by invoking
        /// the <see cref="ITemplateInstanceRegistry.RegisterTemplate(string,System.Func{Intent.Engine.IOutputTarget,Intent.Templates.ITemplate})"/>
        /// on the provided <paramref name="registry"/> parameter.
        /// </para>
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="application"></param>
        protected abstract void Register(ITemplateInstanceRegistry registry, IApplication application);

        /// <summary>
        /// This method indicates that the registration process must not occur for this template.
        /// <para>
        /// Note that this must be called BEFORE the registration process has begun.
        /// </para>
        /// </summary>
        protected virtual void AbortRegistration()
        {
            if (_doRegistrationCalled)
            {
                throw new InvalidOperationException($"{nameof(AbortRegistration)} cannot be called after template registration has already occurred.");
            }
            _registrationAborted = true;
        }
    }
}