using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.Api
{
    public static class AWSAPIGatewayRouteIntegrationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<AWSAPIGatewayRouteTargetModel> IntegrationTarget(this AWSAPIGatewayEndpointModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AWSAPIGatewayRouteIntegrationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => AWSAPIGatewayRouteIntegrationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<AWSAPIGatewayRouteTriggerModel> Trigger(this AWSLambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == AWSAPIGatewayRouteIntegrationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => AWSAPIGatewayRouteIntegrationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}