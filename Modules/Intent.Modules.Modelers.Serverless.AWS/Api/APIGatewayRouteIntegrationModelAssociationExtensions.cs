using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    public static class APIGatewayRouteIntegrationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<APIGatewayRouteIntegrationTargetEndModel> IntegrationTarget(this APIGatewayEndpointModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == APIGatewayRouteIntegrationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => APIGatewayRouteIntegrationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<APIGatewayRouteIntegrationSourceEndModel> Trigger(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == APIGatewayRouteIntegrationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => APIGatewayRouteIntegrationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}