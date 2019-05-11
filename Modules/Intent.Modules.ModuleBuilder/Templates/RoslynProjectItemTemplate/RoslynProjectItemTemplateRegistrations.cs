using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metaDataManager;

        public RoslynProjectItemTemplateRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new RoslynProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IClass> GetModels(Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsCSharpTemplate())
                .ToList();
        }
    }
}
