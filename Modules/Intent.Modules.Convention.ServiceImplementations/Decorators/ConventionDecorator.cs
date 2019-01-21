using Intent.MetaModel.Domain;
using Intent.MetaModel.Service;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Engine;
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

    public class ConventionDecorator : ServiceImplementationDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Conventions.ServiceImplementations";

        private readonly IMetaDataManager _metaDataManager;
        private IEnumerable<string> _stereotypeNames;

        public ConventionDecorator(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public void Configure(IDictionary<string, string> settings)
        {
            var createOnStereotypeValues = settings["Create On Stereotype"];
            _stereotypeNames = createOnStereotypeValues.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override IEnumerable<ConstructorParameter> GetConstructorDependencies(IServiceModel serviceModel)
        {
            return base.GetConstructorDependencies(serviceModel);
        }

        private IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            var allModels = _metaDataManager.GetDomainModels(application);
            var filteredModels = allModels.Where(p => _stereotypeNames.Any(q => p.HasStereotype(q)));

            if (!filteredModels.Any())
            {
                return allModels;
            }

            return filteredModels;
        }
    }
}
