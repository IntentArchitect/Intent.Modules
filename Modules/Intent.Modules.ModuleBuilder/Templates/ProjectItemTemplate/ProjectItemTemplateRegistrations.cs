using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IElement>
    {
        private readonly IMetadataManager _metaDataManager;

        public ProjectItemTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => ProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IElement model)
        {
            return new ProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IElement> GetModels(Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsFileTemplate())
                .ToList();
        }
    }
}
