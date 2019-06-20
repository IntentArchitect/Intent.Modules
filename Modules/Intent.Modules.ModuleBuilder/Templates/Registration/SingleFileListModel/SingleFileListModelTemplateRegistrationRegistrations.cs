using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    public class SingleFileListModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IElement>
    {
        private readonly IMetadataManager _metaDataManager;

        public SingleFileListModelTemplateRegistrationRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => SingleFileListModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElement model)
        {
            return new SingleFileListModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IElement> GetModels(Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetRegistrationType() == RegistrationType.SingleFileListModel)
                .ToList();
        }
    }
}