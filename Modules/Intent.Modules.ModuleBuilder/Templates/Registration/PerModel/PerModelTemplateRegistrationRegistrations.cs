using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.PerModel
{
    public class PerModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IAttribute>
    {
        private readonly IMetaDataManager _metaDataManager;

        public PerModelTemplateRegistrationRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => PerModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IAttribute model)
        {
            return new PerModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IAttribute> GetModels(Intent.SoftwareFactory.Engine.IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .SelectMany(x => x.Attributes)
                .Where(x => true)
                .ToList();
        }
    }
}