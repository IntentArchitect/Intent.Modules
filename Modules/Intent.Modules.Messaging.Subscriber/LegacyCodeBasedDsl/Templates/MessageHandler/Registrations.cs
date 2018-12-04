using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.MetaModels.Common;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
#pragma warning disable 618 // Old code based DSL

namespace Intent.Modules.Messaging.Subscriber.LegacyCodeBasedDsl.Templates.MessageHandler
{
    [Description(MessageHandlerTemplate.IDENTIFIER)]
    public class Registrations : ModelTemplateRegistrationBase<TypeModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => MessageHandlerTemplate.IDENTIFIER;
        public override ITemplate CreateTemplateInstance(IProject project, TypeModel model)
        {
            return new MessageHandlerTemplate(project, model);
        }

        public override IEnumerable<TypeModel> GetModels(IApplication applicationManager)
        {
            var applicationModel = _metaDataManager.GetMetaData<ApplicationModel>(new MetaDataIdentifier("Application")).FirstOrDefault(x => x.Name == applicationManager.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ applicationManager.ApplicationName }]");
                return new TypeModel[0];
            }

            return applicationModel.EventingModel.Subscribing.SubscribedEvents;
        }
    }
}
