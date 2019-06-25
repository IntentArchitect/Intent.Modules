using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Templates;


namespace Intent.Modules.Entities.Repositories.Api.Templates.EntitySpecification
{
    [Description(EntitySpecificationTemplate.Identifier)]
    public class EntitySpecificationTemplateRegistration : ModelTemplateRegistrationBase<IClass>
    {
        private readonly DomainMetadataProvider _metadataManager;
        private IEnumerable<string> _stereotypeNames;

        public EntitySpecificationTemplateRegistration(DomainMetadataProvider metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EntitySpecificationTemplate.Identifier;

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);

            var createOnStereotypeValues = settings["Create On Stereotype"];
            _stereotypeNames = createOnStereotypeValues.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new EntitySpecificationTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Engine.IApplication application)
        {
            var allModels = _metadataManager.GetClasses(application);
            var filteredModels = allModels.Where(p => _stereotypeNames.Any(p.HasStereotype));

            if (!filteredModels.Any())
            {
                return allModels;
            }

            return filteredModels;
        }
    }
}
