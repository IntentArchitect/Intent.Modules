using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Metadata.ApiGateway.Api
{
    public static class RouteAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DownstreamEndModel> DownstreamEndpoints(this ApiGatewayRouteModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RouteAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => RouteAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpstreamEndModel> UpstreamEndpoints(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RouteAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RouteAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpstreamEndModel> UpstreamEndpoints(this QueryModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RouteAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RouteAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UpstreamEndModel> UpstreamEndpoints(this OperationModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == RouteAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => RouteAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}