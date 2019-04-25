using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    [Description(WebApiControllerTemplate.Identifier)]
    public class WebApiControllerTemplateRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public WebApiControllerTemplateRegistrations(IMetaDataManager metaDataManager)
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

