using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.PagedList
{
    [Description(PagedListTemplate.Identifier)]
    public class PagedListTemplateRegistration : NoModelTemplateRegistrationBase
    {
        private readonly IMetadataManager _metadataManager;

        public PagedListTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => PagedListTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new PagedListTemplate(project);
        }
    }
}
