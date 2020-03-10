using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Helpers;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    public class CustomTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IFileTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public CustomTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CustomTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IFileTemplate model)
        {
            return new CustomTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IFileTemplate> GetModels(IApplication application)
        {
            return _metadataManager.GetTemplateDefinitions(application)
                .Where(x => x.GetCreationMode() == CreationMode.Custom)
                .ToList();
        }
    }
}
