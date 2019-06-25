using System.Linq;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Registrations;


namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecRegistrations : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public IModSpecRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => IModSpecTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registery, Engine.IApplication applicationManager)
        {
            var model = _metadataManager.GetClassModels(applicationManager, "Module Builder").ToArray();
            registery.Register(TemplateId, project => new IModSpecTemplate(TemplateId, project, model));
        }
    }
}
