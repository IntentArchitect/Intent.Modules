using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.AWS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.SQS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSSQSDiagramExtensionsModel : DiagramModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AWSSQSDiagramExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<SQSQueueModel> SQSQueues => _element.ChildElements
            .GetElementsOfType(SQSQueueModel.SpecializationTypeId)
            .Select(x => new SQSQueueModel(x))
            .ToList();

    }
}