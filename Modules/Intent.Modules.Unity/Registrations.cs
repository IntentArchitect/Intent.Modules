using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.Unity
{
    public class Registrations : OldProjectTemplateRegistration
    {

        public Registrations()
        {
        }

        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

            RegisterTemplate(UnityConfigTemplate.Identifier, project => new UnityConfigTemplate(project, application.EventDispatcher));
        }
    }
}
