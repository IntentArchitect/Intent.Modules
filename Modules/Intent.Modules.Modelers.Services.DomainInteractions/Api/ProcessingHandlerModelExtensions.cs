using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modelers.Services.DomainInteractions.Api;

public static class ProcessingHandlerModelExtensions
{
    public static IList<ProcessingActionModel> ProcessingActions(this IProcessingHandlerModel model)
    {
        return model.InternalElement.ChildElements.GetElementsOfType(ProcessingActionModel.SpecializationTypeId)
            .Select(x => x.AsProcessingActionModel())
            .ToList();
    }
}