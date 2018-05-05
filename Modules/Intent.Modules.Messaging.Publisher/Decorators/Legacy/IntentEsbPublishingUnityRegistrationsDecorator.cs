using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Messaging.Publisher.Decorators.Legacy
{
    public class IntentEsbPublishingUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.Messaging.Publisher.UnityDecorator.Legacy";
        private readonly IApplication _application;
        private readonly EventingModel _eventingModel;

        public IntentEsbPublishingUnityRegistrationsDecorator(IApplication application, EventingModel eventingModel)
        {
            _application = application;
            _eventingModel = eventingModel;
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                @"using Intent.Esb.Client.Publishing;",
                @"using Intent.Framework.Unity;",
                @"using System.Configuration;",
                @"using Inoxico.Seedwork.Distribution.Esb;"
            };
        }

        public string Registrations()
        {
            const string indentation = "            ";
            var appName = _application.ApplicationName;

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($@"{indentation}// ESB Publishing Config");
            sb.AppendLine($@"{indentation}var businessQueueFactory = new BusinessQueueFactory(new MessageQueueFactory(");
            sb.AppendLine($@"{indentation}    boundedContextName: ""{appName} Application"",");
            sb.AppendLine($@"{indentation}    notifyQueueManagerFactory: new HttpNotifyQueueManagerFactory(esbUrl: ConfigurationManager.AppSettings[""Esb.Url""])));");
            foreach (var publishingQueue in _eventingModel.PublishingQueues)
            {
                if (!publishingQueue.PublishedEvents.Any())
                {
                    continue;
                }

                var queueName = publishingQueue.GetOutputQueueName(appName);
                var tableName = publishingQueue.GetOutputTableName(appName);

                sb.AppendLine($@"{indentation}businessQueueFactory.RegisterQueue(");
                sb.AppendLine($@"{indentation}    name: ""{queueName}"",");
                sb.AppendLine($@"{indentation}    dbConnectionString: ""{appName}DB"",");
                sb.AppendLine($@"{indentation}    dbSchemaName: ""{appName}"",");
                sb.AppendLine($@"{indentation}    dbTableName: ""{tableName}"",");
                sb.AppendLine($@"{indentation}    messageTypes: new[]");
                sb.AppendLine($@"{indentation}    {{");

                foreach (var publishedEvent in publishingQueue.PublishedEvents)
                {
                    sb.AppendLine($@"{indentation}        typeof({publishedEvent.FullName}),");
                }

                sb.AppendLine($@"{indentation}    }});");
            }

            sb.AppendLine($@"{indentation}container.RegisterType<IBusinessQueue>(new PerServiceCallLifetimeManager(), new InjectionFactory(c => businessQueueFactory.Create()));");
            sb.AppendLine($@"{indentation}container.RegisterInstance(businessQueueFactory);");

            return sb.ToString();
        }

        public void PreProcess()
        {
            _application.EventDispatcher.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                {"Key", "Esb.Url"},
                {"Value", $"http://localhost:9000/" }
            });
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentEsbClient,
            };
        }
    }
}