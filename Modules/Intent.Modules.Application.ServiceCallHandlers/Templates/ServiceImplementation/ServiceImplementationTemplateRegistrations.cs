using Intent.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceImplementation
{
    [Description(ServiceImplementationTemplate.Identifier)]
    public class ServiceImplementationTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly ServicesMetadataProvider _metadataProvider;

        public ServiceImplementationTemplateRegistrations(ServicesMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public override string TemplateId => ServiceImplementationTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new ServiceImplementationTemplate(project, model);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            return _metadataProvider.GetServices(application.Id);
        }
    }
}

