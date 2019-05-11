using System.Linq;
using Intent.Modules.Common;
using Intent.Engine;
using Intent.Registrations;


namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecRegistrations : IProjectTemplateRegistration
    {
        private readonly IMetadataManager _metaDataManager;

        public IModSpecRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => IModSpecTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registery, Engine.IApplication applicationManager)
        {
            var model = _metaDataManager.GetClassModels(applicationManager, "Module Builder").ToArray();
            registery.Register(TemplateId, project => new IModSpecTemplate(TemplateId, project, model));
        }
    }
}
