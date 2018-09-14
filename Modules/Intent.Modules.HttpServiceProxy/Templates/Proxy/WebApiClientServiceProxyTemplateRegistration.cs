using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.HttpServiceProxy.Templates.Proxy
{
    [Description(WebApiClientServiceProxyTemplate.IDENTIFIER)]
    public class WebApiClientServiceProxyTemplateRegistration : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public WebApiClientServiceProxyTemplateRegistration(IMetaDataManager metaDataManager)
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
            var results = _metaDataManager.GetMetaData<IServiceModel>(new MetaDataIdentifier("Service"))
                .Where(x => x.GetStereotypeProperty("Consumers", "CommaSeperatedList", "").Split(',').Any(y => y.Trim().Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return results;
        }
    }
}

