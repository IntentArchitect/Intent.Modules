using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSLambdaFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSLambdaFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<LambdaFunctionModel> LambdaFunctions => _element.ChildElements
            .GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
            .Select(x => new LambdaFunctionModel(x))
            .ToList();

    }
}