using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSLambdaPackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSLambdaPackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<LambdaFunctionModel> LambdaFunctions => UnderlyingPackage.ChildElements
            .GetElementsOfType(LambdaFunctionModel.SpecializationTypeId)
            .Select(x => new LambdaFunctionModel(x))
            .ToList();

    }
}