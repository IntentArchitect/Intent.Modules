using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Api
{
    public interface ICSharpTemplate : ITemplateRegistration
    {
    }
}