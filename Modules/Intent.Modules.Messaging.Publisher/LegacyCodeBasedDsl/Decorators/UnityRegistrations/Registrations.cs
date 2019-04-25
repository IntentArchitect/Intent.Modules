using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.Application;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Messaging.Publisher.LegacyCodeBasedDsl.Decorators.UnityRegistrations
{
    [Description(UnityRegistrationsDecorator.IDENTIFIER)]
    public class Registrations : DecoratorRegistration<IUnityRegistrationsDecorator>
    {
        private readonly IMetadataManager _metaDataManager;

        public Registrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string DecoratorId => UnityRegistrationsDecorator.IDENTIFIER;

        public override object CreateDecoratorInstance(IApplication application)
        {
            var applicationModel = _metaDataManager.GetMetaData<ApplicationModel>(new MetaDataIdentifier("Application")).FirstOrDefault(x => x.Name == application.ApplicationName);
            if (applicationModel == null)
            {
                Logging.Log.Warning($"ApplicationModel could not be found for application [{ application.ApplicationName }]");
            }


            return new UnityRegistrationsDecorator(application, applicationModel?.EventingModel);
        }
    }
}
