using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public ApiVersionModel ApiVersion => UnderlyingPackage.ChildElements
            .GetElementsOfType(ApiVersionModel.SpecializationTypeId)
            .Select(x => new ApiVersionModel(x))
            .SingleOrDefault();

    }
}