using Intent.Modules.Messaging.Subscriber.Legacy.MessageHandler;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Messaging.Subscriber.Legacy.Decorators
{
    public class IntentEsbConsumingUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Messaging.Subscriber.UnityRegistrationsDecorator.Legacy";
        private readonly IApplication _application;
        private readonly EventingModel _eventingModel;

        public IntentEsbConsumingUnityRegistrationsDecorator(IApplication application, EventingModel eventingModel)
        {
            _application = application;
            _eventingModel = eventingModel;
        }

        public IEnumerable<string> DeclareUsings()
        {
            var usings = _eventingModel.Subscribing.SubscribedEvents
                .Select(x => $"using {x.NS};")
                .Distinct()
                .ToList();

            usings.Add("using Intent.Esb.Client.Consuming;");

            return usings;
        }

        public string Registrations()
        {
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(MessageHandlerTemplate.Identifier),
            };
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.AkkaRemote,
                NugetPackages.IntentEsbClient,
            };
        }
    }
}