using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    public class SingleFileNoModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IElement>
    {
        private readonly IMetadataManager _metaDataManager;

        public SingleFileNoModelTemplateRegistrationRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => SingleFileNoModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElement model)
        {
            return new SingleFileNoModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IElement> GetModels(Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetRegistrationType() == RegistrationType.SingleFileNoModel)
                .ToList();
        }
    }
}