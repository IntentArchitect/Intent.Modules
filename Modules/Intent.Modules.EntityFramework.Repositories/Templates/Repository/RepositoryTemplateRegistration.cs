using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Templates;


namespace Intent.Modules.EntityFramework.Repositories.Templates.Repository
{
    [Description(RepositoryTemplate.Identifier)]
    public class RepositoryTemplateRegistration : ModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<string> _stereotypeNames;

        public RepositoryTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RepositoryTemplate.Identifier;

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);

            var createOnStereotypeValues = settings["Create On Stereotype"];
            _stereotypeNames = createOnStereotypeValues.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ITemplate CreateTemplateInstance(IProject project, ClassModel model)
        {
            return new RepositoryTemplate(model, project);
        }

        public override IEnumerable<ClassModel> GetModels(Engine.IApplication application)
        {
            var allModels = _metadataManager.Domain(application).GetClassModels();
            var filteredModels = allModels.Where(p => _stereotypeNames.Any(p.HasStereotype));

            if (!filteredModels.Any())
            {
                return allModels;
            }

            return filteredModels;
        }
    }
}
