using Intent.Templates;
using System;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Registrations;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    [Description(ServiceCallHandlerImplementationTemplate.Identifier)]
    public class ServiceCallHandlerImplementationTemplateRegistrations : IProjectTemplateRegistration
    {
        private readonly ServicesMetadataProvider _metadataProvider;

        public ServiceCallHandlerImplementationTemplateRegistrations(ServicesMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public string TemplateId => ServiceCallHandlerImplementationTemplate.Identifier;


        private ITemplate CreateTemplateInstance(IProject project, IServiceModel serviceModel, IOperation operationModel)
        {
            return new ServiceCallHandlerImplementationTemplate(project, serviceModel, operationModel);
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var serviceModels = _metadataProvider.GetServices(applicationManager);
            foreach (var serviceModel in serviceModels)
            {
                foreach (var operationModel in serviceModel.Operations)
                {
                    registry.Register(TemplateId, (project) => CreateTemplateInstance(project, serviceModel, operationModel));
                }
            }
        }
    }
}

