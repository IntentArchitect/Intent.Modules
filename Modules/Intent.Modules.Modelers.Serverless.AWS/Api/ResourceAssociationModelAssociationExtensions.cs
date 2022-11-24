using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    public static class ResourceAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationTargetEndModel> ResourceAssociationTargetEnd(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ResourceAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationTargetEndModel> ResourceAssociationTargetEnd(this SQSQueueModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ResourceAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationSourceEndModel> ResourceAssociationSourceEnd(this SQSQueueModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ResourceAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationSourceEndModel> ResourceAssociationSourceEnd(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ResourceAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ResourceAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationEndModel> ResourceAssociationEnds(this SQSQueueModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsResourceAssociationEndModel())
                .Select(ResourceAssociationEndModel.Create)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ResourceAssociationEndModel> ResourceAssociationEnds(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsResourceAssociationEndModel())
                .Select(ResourceAssociationEndModel.Create)
                .ToList();
        }

    }
}