using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Messaging.Publisher.Decorators.Legacy
{
    public class IntentEsbPublishingUnityRegistrationsDecorator : IUnityRegistrationsDecorator, IHasNugetDependencies
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
                @"using Intent.Framework.Unity;"
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
            sb.AppendLine($@"{indentation}var businessQueueFactory = new BusinessQueueFactory(boundedContextName: ""{appName} Application"", esbLocation: ""akka.tcp://intentEsb@localhost:9391"");");

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

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentEsbClient,
            };
        }
    }
}