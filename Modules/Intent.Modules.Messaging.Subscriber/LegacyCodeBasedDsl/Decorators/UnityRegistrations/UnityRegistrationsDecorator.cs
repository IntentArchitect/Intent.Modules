using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Templates.MessageHandler;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.Templates

namespace Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Decorators.UnityRegistrations
{
    public class UnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string IDENTIFIER = "Intent.Messaging.LegacyCodeBasedDsl.Subscriber.UnityRegistrationsDecorator";
        private readonly IApplication _application;
        private readonly EventingModel _eventingModel;

        public UnityRegistrationsDecorator(IApplication application, EventingModel eventingModel)
        {
            _application = application;
            _eventingModel = eventingModel;
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (_eventingModel == null)
            {
                return new string[0];
            }

            var usings = _eventingModel.Subscribing.SubscribedEvents
                .Select(x => $"{x.NS}")
                .Distinct()
                .ToList();

            usings.Add("Intent.Esb.Client.Consuming");

            return usings;
        }

        public string Registrations()
        {
            if (_eventingModel == null)
            {
                return string.Empty;
            }

            var output = $@"

            // ESB Consuming Config
            container.RegisterInstance(new MessageDispatcher(""{_application.ApplicationName}DB"", ""{_application.ApplicationName}"", ""_EventingMessageReceived"", (t, s) => container.Resolve(t, s)));";
            foreach (var subscribedEvent in _eventingModel.Subscribing.SubscribedEvents)
            {
                output += $@"
            container.RegisterType<IMessageHandler, { subscribedEvent.TypeName }Handler>(typeof({ subscribedEvent.TypeName }).FullName);";
            }

            return output;
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            if (_eventingModel == null)
            {
                return new ITemplateDependency[0];
            }

            return new[]
            {
                TemplateDependancy.OnTemplate(MessageHandlerTemplate.IDENTIFIER),
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            if (_eventingModel == null)
            {
                return new INugetPackageInfo[0];
            }

            return new[]
            {
                NugetPackages.IntentEsbClient,
            };
        }
    }
}