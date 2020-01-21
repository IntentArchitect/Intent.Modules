using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFramework.Repositories.Templates.RepositoryBase;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase
{
    [Description(RepositoryBaseTemplate.Identifier)]
    public class RepositoryBaseTemplateRegistration : NoModelTemplateRegistrationBase
    {
        private readonly IMetadataManager _metadataManager;

        public RepositoryBaseTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RepositoryBaseTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new RepositoryBaseTemplate(project);
        }
    }
}
