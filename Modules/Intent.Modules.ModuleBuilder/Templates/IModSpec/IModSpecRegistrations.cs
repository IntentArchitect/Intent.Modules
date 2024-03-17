using Intent.Engine;
using Intent.ModuleBuilder.Api;
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

        public void DoRegistration(ITemplateInstanceRegistry registry, Engine.IApplication application)
        {
            foreach (var intentModule in _metadataManager.ModuleBuilder(application).GetIntentModuleModels())
            {
                registry.RegisterTemplate(TemplateId, context => new IModSpecTemplate(
                    project: context,
                    model: intentModule,
                    metadataManager: _metadataManager));
            }
        }
    }
}
