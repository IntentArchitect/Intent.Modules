using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IAttribute>
    {
        private readonly IMetaDataManager _metaDataManager;

        public RoslynProjectItemTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IAttribute model)
        {
            return new RoslynProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IAttribute> GetModels(SoftwareFactory.Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .SelectMany(x => x.Attributes)
                .Where(x => true)
                .ToList();
        }
    }
}
