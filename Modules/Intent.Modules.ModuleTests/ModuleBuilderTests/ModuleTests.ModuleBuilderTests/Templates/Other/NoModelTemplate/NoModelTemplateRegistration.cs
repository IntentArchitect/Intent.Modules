using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Other.NoModelTemplate
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class NoModelTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => NoModelTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new NoModelTemplate(project, null);
        }
    }
}