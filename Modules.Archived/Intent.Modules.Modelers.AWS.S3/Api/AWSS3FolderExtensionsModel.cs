using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.S3.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSS3FolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSS3FolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<S3BucketModel> S3Buckets => _element.ChildElements
            .GetElementsOfType(S3BucketModel.SpecializationTypeId)
            .Select(x => new S3BucketModel(x))
            .ToList();

    }
}