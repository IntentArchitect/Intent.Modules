using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates;

using System;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    [Description(ServiceCallHandlerImplementationTemplate.Identifier)]
    public class ServiceCallHandlerImplementationTemplateRegistrations : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metaDataManager;

        public ServiceCallHandlerImplementationTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => ServiceCallHandlerImplementationTemplate.Identifier;


        private ITemplate CreateTemplateInstance(IProject project, IServiceModel serviceModel, IOperationModel operationModel)
        {
            return new ServiceCallHandlerImplementationTemplate(project, serviceModel, operationModel);
        }

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            var serviceModels = _metaDataManager.GetServiceModels(applicationManager);
            foreach (var serviceModel in serviceModels)
            {
                foreach (var operationModel in serviceModel.Operations)
                {
                    registery.Register(TemplateId, (project) => CreateTemplateInstance(project, serviceModel, operationModel));
                }
            }
        }
    }
}

