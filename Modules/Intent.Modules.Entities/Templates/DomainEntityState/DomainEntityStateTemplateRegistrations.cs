using System.Collections.Generic;
using System.ComponentModel;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    [Description(DomainEntityStateTemplate.Identifier)]
    public class DomainEntityStateTemplateRegistrations : ModelTemplateRegistrationBase<IClass>
    {
        private readonly IMetaDataManager _metaDataManager;

        public DomainEntityStateTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => DomainEntityStateTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IClass model)
        {
            return new DomainEntityStateTemplate(model, project);
        }

        public override IEnumerable<IClass> GetModels(Intent.SoftwareFactory.Engine.IApplication application)
        {
            return _metaDataManager.GetDomainModels(application);
        }
    }
}
