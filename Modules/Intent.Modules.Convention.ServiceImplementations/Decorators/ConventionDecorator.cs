using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Convention.ServiceImplementations.Decorators
{
    // DJVV - TODO:
    // Convention module:
    // 1. Scan MetadataManger in the same way the Repo Interface register does it
    // 2. This will mean that you will need the module settings like the Repo Interface module (filtering and TemplateID)
    // 3. Then you can return those interface FQDNs with parameter names for the Constructor dependencies
    // 4. Determining usings are dependent on the current service implementation
    // 5. Now you can match up domain classes with services and populate the relevant implementations
    // 6. Mapping incoming and outgoing DTOs/Domains

    public class ConventionDecorator : ServiceImplementationDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Conventions.ServiceImplementations.Decorator";

        private readonly IMetaDataManager _metaDataManager;
        private readonly Intent.SoftwareFactory.Engine.IApplication _application;
        private string _repositoryInterfaceTemplateId;

        public ConventionDecorator(IMetaDataManager metaDataManager, Intent.SoftwareFactory.Engine.IApplication application)
        {
            _metaDataManager = metaDataManager;
            _application = application;
        }

        public void Configure(IDictionary<string, string> settings)
        {
            _repositoryInterfaceTemplateId = settings["Repository Interface Template Id"];
        }

        public override IEnumerable<string> GetUsings(IServiceModel serviceModel)
        {
            var currentDomain = GetDomainForService(serviceModel);

            if (currentDomain == null)
            {
                return new List<string>();
            }

            var repoInterfaceTemplate = _application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<IClass>(_repositoryInterfaceTemplateId, p => p.Id == currentDomain.Id));
            if (repoInterfaceTemplate == null)
            {
                return new List<string>();
            }

            return new List<string>
            {
                repoInterfaceTemplate.Namespace
            };
        }

        public override IEnumerable<ConstructorParameter> GetConstructorDependencies(IServiceModel serviceModel)
        {
            var currentDomain = GetDomainForService(serviceModel);

            if (currentDomain == null)
            {
                return new List<ConstructorParameter>();
            }

            var repoInterfaceTemplate = _application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<IClass>(_repositoryInterfaceTemplateId, p => p.Id == currentDomain.Id));
            if (repoInterfaceTemplate == null)
            {
                return new List<ConstructorParameter>();
            }

            var paramName = repoInterfaceTemplate.ClassName.Remove(0, 1).ToCamelCase();
            var paramType = $"{repoInterfaceTemplate.Namespace}.{repoInterfaceTemplate.ClassName}";

            return new List<ConstructorParameter>
            {
                new ConstructorParameter(paramType, paramName)
            };
        }

        private IClass GetDomainForService(IServiceModel serviceModel)
        {
            var lowerServiceName = serviceModel.Name.ToLower();
            var domains = _metaDataManager.GetDomainModels(_application);
            return domains
                .SingleOrDefault(p =>
                {
                    var lowerDomainName = p.Name.ToLower();
                    return lowerDomainName == lowerServiceName
                    || (lowerDomainName + "service") == lowerServiceName
                    || (lowerDomainName + "manager") == lowerServiceName;
                });
        }
    }
}
