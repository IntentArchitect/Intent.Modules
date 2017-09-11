using System;
using System.Diagnostics;
using System.Linq;
using Intent.Packages.Application.ServiceCallHandlers.Legacy.ServiceCallHandler;
using Intent.Packages.Application.ServiceCallHandlers.Legacy.ServiceImplementation;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Application.ServiceCallHandlers
{
    public class Registrations : OldProjectTemplateRegistration
    {
        public override void RegisterStuff(IApplication application, IMetaDataManager metaDataManager)
        {
            var serviceModels = metaDataManager.GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy")).Where(x => x.ApplicationName == application.ApplicationName).ToList();

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
