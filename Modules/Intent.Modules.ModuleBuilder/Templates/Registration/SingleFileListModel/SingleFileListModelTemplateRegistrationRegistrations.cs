using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    public class SingleFileListModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IModuleBuilderElement>
    {
        private readonly IMetadataManager _metadataManager;

        public SingleFileListModelTemplateRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SingleFileListModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IModuleBuilderElement model)
        {
            return new SingleFileListModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IModuleBuilderElement> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetAllElements(applicationManager)
                .Where(x => x.IsTemplate() && x.GetCreationMode() == CreationMode.SingleFileListModel)
                .ToList();
        }
    }
}