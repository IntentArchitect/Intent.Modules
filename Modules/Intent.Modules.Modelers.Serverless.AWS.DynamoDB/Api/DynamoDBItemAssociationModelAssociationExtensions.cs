using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.DynamoDB.Api
{
    public static class DynamoDBItemAssociationModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<DynamoDBItemAssociationTargetEndModel> DynamoDbItemAssociationTargetEnd(this DynamoDBItemModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DynamoDBItemAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DynamoDBItemAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DynamoDBItemAssociationTargetEndModel> DynamoDbItemAssociationTargetEnd(this DynamoDBMapAttributeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DynamoDBItemAssociationModel.SpecializationType && x.IsTargetEnd())
                .Select(x => DynamoDBItemAssociationModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DynamoDBItemAssociationSourceEndModel> DynamoDbItemAssociationSourceEnd(this DynamoDBMapAttributeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == DynamoDBItemAssociationModel.SpecializationType && x.IsSourceEnd())
                .Select(x => DynamoDBItemAssociationModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<DynamoDBItemAssociationEndModel> DynamoDBItemAssociationEnds(this DynamoDBMapAttributeModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.IsDynamoDBItemAssociationEndModel())
                .Select(DynamoDBItemAssociationEndModel.Create)
                .ToList();
        }

    }
}