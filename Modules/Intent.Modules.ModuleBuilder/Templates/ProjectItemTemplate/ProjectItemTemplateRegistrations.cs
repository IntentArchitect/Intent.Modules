using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplate
{
    public class ProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public ProjectItemTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => ProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new ProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IClass> GetModels(IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsFileTemplate())
                .ToList();
        }
    }
}
