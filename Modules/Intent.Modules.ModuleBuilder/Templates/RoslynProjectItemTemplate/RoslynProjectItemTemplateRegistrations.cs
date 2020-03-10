using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplate
{
    public class RoslynProjectItemTemplateRegistrations : ModelTemplateRegistrationBase<IFileTemplate>
    {
        private readonly IMetadataManager _metadataManager;

        public RoslynProjectItemTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IFileTemplate model)
        {
            return new RoslynProjectItemTemplateTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IFileTemplate> GetModels(IApplication application)
        {
            return _metadataManager.GetCSharpTemplates(application)
                .ToList();
        }
    }
}
