using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.SQS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSSQSFolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSSQSFolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<SQSQueueModel> SQSQueues => _element.ChildElements
            .GetElementsOfType(SQSQueueModel.SpecializationTypeId)
            .Select(x => new SQSQueueModel(x))
            .ToList();

    }
}