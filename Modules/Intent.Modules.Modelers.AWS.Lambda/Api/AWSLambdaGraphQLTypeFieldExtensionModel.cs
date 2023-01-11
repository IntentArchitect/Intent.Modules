using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.AppSync.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSLambdaGraphQLTypeFieldExtensionModel : TypeFieldModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSLambdaGraphQLTypeFieldExtensionModel(IElement element) : base(element)
        {
        }

    }
}