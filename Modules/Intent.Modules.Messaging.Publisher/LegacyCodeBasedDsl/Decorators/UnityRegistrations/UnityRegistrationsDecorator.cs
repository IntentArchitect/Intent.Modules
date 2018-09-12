using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Modules.Constants;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Messaging.Publisher.LegacyCodeBasedDsl.Decorators.UnityRegistrations
{
    public class UnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies, IRequiresPreProcessing, IHasAssemblyDependencies
    {
        public const string IDENTIFIER = "Intent.Messaging.LegacyCodeBasedDsl.Publisher.UnityDecorator";
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

            return new[]
            {
                "System.Configuration",
                "Inoxico.Seedwork.Distribution.Esb",
                "Intent.Esb.Client.Publishing",
                "Unity.Injection"
            };
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Configuration") 
            };
        }

        public string Registrations()
        {
            if (_eventingModel == null)
            {
                return string.Empty;
            }

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
            if (_eventingModel == null)
            {
                return;
            }

            _application.EventDispatcher.Publish(ApplicationEvents.Config_AppSetting, new Dictionary<string, string>()
            {
                {"Key", "Esb.Url"},
                {"Value", "http://localhost:9000/" }
            });
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            if (_eventingModel == null)
            {
                return new INugetPackageInfo[0];
            }

            return new[]
            {
                NugetPackages.IntentEsbClient
            };
        }
    }
}