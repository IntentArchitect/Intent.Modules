using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates


namespace Intent.Modules.HttpServiceProxy.Templates.Proxy
{
    [Description(WebApiClientServiceProxyTemplate.IDENTIFIER)]
    public class WebApiClientServiceProxyTemplateRegistration : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public WebApiClientServiceProxyTemplateRegistration(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => WebApiClientServiceProxyTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new WebApiClientServiceProxyTemplate(project, model);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            var results = _metaDataManager.GetMetaData<IServiceModel>("Services")
                .Where(x => x.GetStereotypeProperty("Consumers", "CommaSeperatedList", "").Split(',').Any(y => y.Trim().Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return results;
        }
    }
}

