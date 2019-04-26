using Intent.Modelers.Services.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
{
    [Description(WebApiControllerTemplate.Identifier)]
    public class WebApiControllerTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public WebApiControllerTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => WebApiControllerTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new WebApiControllerTemplate(project, model);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetServiceModels(application);
        }
    }
}

