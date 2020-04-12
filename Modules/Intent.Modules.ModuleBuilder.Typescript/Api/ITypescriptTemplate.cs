using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Typescript.Api
{
    public interface ITypescriptTemplate : ITemplateRegistration
    {
    }
}