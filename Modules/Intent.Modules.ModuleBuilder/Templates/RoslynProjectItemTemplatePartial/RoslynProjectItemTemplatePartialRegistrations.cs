using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    public class RoslynProjectItemTemplatePartialRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetadataManager _metadataManager;

        public RoslynProjectItemTemplatePartialRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RoslynProjectItemTemplatePartialTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new RoslynProjectItemTemplatePartialTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IClass> GetModels(Engine.IApplication applicationManager)
        {
            return _metadataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => x.IsCSharpTemplate())
                .ToList();
        }
    }
}