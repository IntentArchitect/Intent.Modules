using System.Linq;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Registrations;


namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecRegistrations : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public IModSpecRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => IModSpecTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registry, Engine.IApplication applicationManager)
        {
            registry.RegisterTemplate(TemplateId, context => new IModSpecTemplate(
                templateId: TemplateId, 
                project: context, 
                metadataManager:_metadataManager));
        }
    }
}
