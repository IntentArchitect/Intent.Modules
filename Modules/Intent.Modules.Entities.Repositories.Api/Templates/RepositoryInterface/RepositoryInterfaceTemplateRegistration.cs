using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    [Description(RepositoryInterfaceTemplate.Identifier)]
    public class RepositoryInterfaceTemplateRegistration : NoModelTemplateRegistrationBase
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<string> _stereotypeNames;

        public RepositoryInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RepositoryInterfaceTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new RepositoryInterfaceTemplate(project);
        }
    }
}
