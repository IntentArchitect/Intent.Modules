using System.Collections.Generic;
using System.Data;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelPartial", Version = "1.0")]

namespace Intent.Modelers.Services.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DTOModel
    {
        public IElementApplication Application => _element.Application;
    }
}