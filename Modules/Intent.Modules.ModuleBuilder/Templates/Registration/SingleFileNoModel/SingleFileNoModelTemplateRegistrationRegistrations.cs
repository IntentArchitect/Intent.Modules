using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    public class SingleFileNoModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IFileTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public SingleFileNoModelTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SingleFileNoModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IFileTemplate model)
        {
            return new SingleFileNoModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IFileTemplate> GetModels(IApplication application)
        {
            return _metadataManager.GetTemplateDefinitions(application)
                .Where(x => x.GetFileTemplateSettings().CreationMode().IsSingleFileNoModel() && x.GetFileTemplateSettings().ModelType() == null)
                .ToList();
        }
    }
}