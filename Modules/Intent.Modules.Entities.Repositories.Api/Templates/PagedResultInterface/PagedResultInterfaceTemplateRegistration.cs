using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface
{
    [Description(PagedResultInterfaceTemplate.Identifier)]
    public class PagedResultInterfaceTemplateRegistration : NoModelTemplateRegistrationBase
    {
        private readonly IMetadataManager _metadataManager;

        public PagedResultInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => PagedResultInterfaceTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new PagedResultInterfaceTemplate(project);
        }
    }
}
