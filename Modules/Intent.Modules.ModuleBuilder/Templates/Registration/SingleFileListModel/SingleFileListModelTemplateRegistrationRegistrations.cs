using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileListModel
{
    public class SingleFileListModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public SingleFileListModelTemplateRegistrationRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => SingleFileListModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new SingleFileListModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetCreationMode() == CreationMode.SingleFileListModel)
                .ToList();
        }
    }
}