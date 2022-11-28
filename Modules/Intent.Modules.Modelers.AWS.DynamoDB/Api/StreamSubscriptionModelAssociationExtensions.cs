using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Serverless.AWS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    public static class StreamSubscriptionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<StreamSubscriptionTargetEndModel> Lambda(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StreamSubscriptionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => StreamSubscriptionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<StreamSubscriptionSourceEndModel> Table(this DynamoDBTableModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == StreamSubscriptionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => StreamSubscriptionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}