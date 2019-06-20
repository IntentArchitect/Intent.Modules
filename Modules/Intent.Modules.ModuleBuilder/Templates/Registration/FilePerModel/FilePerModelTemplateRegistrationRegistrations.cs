using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel
{
    public class FilePerModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IElement>
    {
        private readonly IMetadataManager _metaDataManager;

        public FilePerModelTemplateRegistrationRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => FilePerModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElement model)
        {
            return new FilePerModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IElement> GetModels(Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetRegistrationType() == RegistrationType.FilePerModel)
                .ToList();
        }
    }
}