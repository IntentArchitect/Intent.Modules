using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ConditionalStatementTypeModel> GetConditionalStatementTypeModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ConditionalStatementTypeModel.SpecializationTypeId)
                .Select(x => new ConditionalStatementTypeModel(x))
                .ToList();
        }
        public static IList<StateMachineModel> GetStateMachineModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(StateMachineModel.SpecializationTypeId)
                .Select(x => new StateMachineModel(x))
                .ToList();
        }

    }
}