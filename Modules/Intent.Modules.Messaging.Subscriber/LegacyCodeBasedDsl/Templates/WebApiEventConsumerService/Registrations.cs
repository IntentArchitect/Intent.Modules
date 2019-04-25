using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Templates.WebApiEventConsumerService
{
    [Description(WebApiEventConsumerServiceTemplate.IDENTIFIER)]
    public class Registrations : ModelTemplateRegistrationBase<SubscribingModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => WebApiEventConsumerServiceTemplate.IDENTIFIER;
        public override ITemplate CreateTemplateInstance(IProject project, SubscribingModel model)
        {
            return new WebApiEventConsumerServiceTemplate(project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<SubscribingModel> GetModels(IApplication applicationManager)
        {
            var applicationModel = _metaDataManager.GetMetaData<ApplicationModel>(new MetaDataIdentifier("Application")).FirstOrDefault(x => x.Name == applicationManager.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ applicationManager.ApplicationName }]");
                return new SubscribingModel[0];
            }

            var subscribingModel = applicationModel.EventingModel.Subscribing;
            if (subscribingModel.ConsumptionChannel != EventConsumptionChannel.WebApi)
            {
                return new SubscribingModel[0];
            }

            return new[]
            {
                subscribingModel
            };
        }
    }
}
