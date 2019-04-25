using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.DDD.Templates.RepositoryInterface
{
    [Description(RepositoryInterfaceTemplate.Identifier)]
    public class RepositoryInterfaceTemplateRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;
        private IEnumerable<string> _stereotypeNames;

        public RepositoryInterfaceTemplateRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => RepositoryInterfaceTemplate.Identifier;

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);

            var createOnStereotypeValues = settings["Create On Stereotype"];
            _stereotypeNames = createOnStereotypeValues.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new RepositoryInterfaceTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
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
