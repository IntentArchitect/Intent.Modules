using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    public class SingleFileNoModelTemplateRegistrationRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public SingleFileNoModelTemplateRegistrationRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => SingleFileNoModelTemplateRegistrationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new SingleFileNoModelTemplateRegistrationTemplate(project, model);
        }

        public override IEnumerable<IClass> GetModels(IApplication applicationManager)
        {
            return _metaDataManager.GetClassModels(applicationManager, "Module Builder")
                .Where(x => (x.IsCSharpTemplate() || x.IsFileTemplate()) && x.GetRegistrationType() == RegistrationType.SingleFileNoModel)
                .ToList();
        }
    }
}