using System.Linq;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class IModSpecRegistrations : IProjectTemplateRegistration
    {
        private readonly IMetaDataManager _metaDataManager;

        public IModSpecRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public string TemplateId => IModSpecTemplate.TemplateId;

        public void DoRegistration(ITemplateInstanceRegistry registery, IApplication applicationManager)
        {
            var model = _metaDataManager.GetClassModels(applicationManager, "Module Builder").ToArray();
            registery.Register(TemplateId, project => new IModSpecTemplate(TemplateId, project, model));
        }
    }
}
