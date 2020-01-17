using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    public class RoslynProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<ITemplateDefinition>
    {
        private readonly IMetadataManager _metadataManager;

        public RoslynProjectItemTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, ITemplateDefinition model)
        {
            return new RoslynProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<ITemplateDefinition> GetModels(IApplication application)
        {
            return _metadataManager.GetCSharpTemplates(application)
                .ToList();
        }
    }
}