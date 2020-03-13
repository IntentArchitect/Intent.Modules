using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class CSharpTemplate : ModuleBuilderElementBase, ICSharpTemplate
    {
        public CSharpTemplate(IElement element) : base(element)
        {
        }
    }
}