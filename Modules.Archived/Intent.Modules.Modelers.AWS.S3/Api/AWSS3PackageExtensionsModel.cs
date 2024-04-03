using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.S3.Api
{
    [IntentManaged(Mode.Merge)]
    public class AWSS3PackageExtensionsModel : AWSPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public AWSS3PackageExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<S3BucketModel> S3Buckets => UnderlyingPackage.ChildElements
            .GetElementsOfType(S3BucketModel.SpecializationTypeId)
            .Select(x => new S3BucketModel(x))
            .ToList();

    }
}