using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<ITemplateDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public RoslynProjectItemTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateDefinition model)
        {
            return new RoslynProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<ITemplateDefinition> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetCSharpTemplates(applicationManager)
                .ToList();
        }
    }
}
