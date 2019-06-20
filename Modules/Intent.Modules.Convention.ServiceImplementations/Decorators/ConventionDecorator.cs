using Humanizer.Inflections;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Convention.ServiceImplementations.MethodImplementationStrategies;
using Intent.Engine;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain;
using Intent.Modelers.Services.Api;
using Intent.Plugins;

namespace Intent.Modules.Convention.ServiceImplementations.Decorators
{
    public class ConventionDecorator : ServiceImplementationDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Conventions.ServiceImplementations.Decorator";

        private readonly IMetadataManager _metaDataManager;
        private readonly Engine.IApplication _application;
        private string _repositoryInterfaceTemplateId;

        public ConventionDecorator(IMetadataManager metaDataManager, Engine.IApplication application)
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

            var repoInterfaceTemplate = _application.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<IMetadataModel>(_repositoryInterfaceTemplateId, p => p.Id == currentDomain.Id));
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

            var templateDepenency = TemplateDependency.OnModel<IMetadataModel>(_repositoryInterfaceTemplateId, p => p.Id == currentDomain.Id);
            var repoInterfaceTemplate = _application.FindTemplateInstance<IHasClassDetails>(templateDepenency);
            if (repoInterfaceTemplate == null)
            {
                return new List<ConstructorParameter>();
            }

            var paramName = repoInterfaceTemplate.ClassName.Remove(0, 1).ToCamelCase();
            var paramType = $"{repoInterfaceTemplate.Namespace}.{repoInterfaceTemplate.ClassName}";

            return new List<ConstructorParameter>
            {
                new ConstructorParameter(paramType, paramName, templateDepenency)
            };
        }

        public override string GetDecoratedImplementation(IServiceModel serviceModel, IOperation operationModel)
        {
            var currentDomain = GetDomainForService(serviceModel);

            if (currentDomain == null)
            {
                return string.Empty;
            }

            return MethodImplementationStrategy.ImplementOnMatch(_metaDataManager, _application, currentDomain, operationModel);
        }

        private Modelers.Domain.Api.IClass GetDomainForService(IServiceModel serviceModel)
        {
            var lowerServiceName = serviceModel.Name.ToLower();
            var domains = _metaDataManager.GetDomainClasses(_application);
            return domains
                .SingleOrDefault(p =>
                {
                    var lowerDomainName = p.Name.ToLower();
                    var pluralLowerDomainName = Vocabularies.Default.Pluralize(lowerDomainName);
                    return lowerDomainName == lowerServiceName
                    || pluralLowerDomainName == lowerServiceName
                    || (lowerDomainName + "service") == lowerServiceName
                    || (lowerDomainName + "manager") == lowerServiceName
                    || (lowerDomainName + "controller") == lowerServiceName;
                });
        }
    }
}
