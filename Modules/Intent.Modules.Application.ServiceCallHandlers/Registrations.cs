using Intent.Modules.Application.ServiceCallHandlers.Legacy.ServiceCallHandler;
using Intent.Modules.Application.ServiceCallHandlers.Legacy.ServiceImplementation;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;
using System.Linq;

namespace Intent.Modules.Application.ServiceCallHandlers
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataIdentifier("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

            foreach (var serviceModel in serviceModels)
            {
                RegisterTemplate(ServiceImplementationTemplate.Identifier, project => new ServiceImplementationTemplate(serviceModel, project));

                foreach (var operationModel in serviceModel.Operations)
                {
                    RegisterTemplate(ServiceCallHandlerImplementationTemplate.Identifier, project => new ServiceCallHandlerImplementationTemplate(serviceModel, operationModel, project));
                }
            }
        }
    }
}
