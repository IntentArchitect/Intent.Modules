using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    public static class APIEndpointIntegrationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<APIEndpointIntegrationTargetEndModel> IntegrationTarget(this APIGatewayEndpointModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == APIEndpointIntegrationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => APIEndpointIntegrationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<APIEndpointIntegrationSourceEndModel> Trigger(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == APIEndpointIntegrationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => APIEndpointIntegrationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}