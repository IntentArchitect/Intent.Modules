using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.IdentityServer4.Templates.IdentityConfig
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class IdentityConfigRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => IdentityConfig.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new IdentityConfig(project, null);
        }
        private readonly IMetaDataManager _metaDataManager;

        public IdentityConfigRegistration(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }
    }
}