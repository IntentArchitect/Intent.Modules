using Intent.Modelers.Domain.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    [Description(DomainEntityInterfaceTemplate.Identifier)]
    public class DomainEntityInterfaceTemplateRegistrations : ModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEntityInterfaceTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEntityInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ClassModel model)
        {
            return new DomainEntityInterfaceTemplate(model, project, _metadataManager);
        }

        public override IEnumerable<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.GetDomainClasses(application.Id);
        }
    }
}
