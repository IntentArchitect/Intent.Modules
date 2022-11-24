using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    public static class SQSSubscriptionModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<SQSSubscriptionTargetEndModel> Lambda(this SQSQueueModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SQSSubscriptionModel.SpecializationType && x.IsTargetEnd())
                .Select(x => SQSSubscriptionModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<SQSSubscriptionSourceEndModel> SqsQueue(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == SQSSubscriptionModel.SpecializationType && x.IsSourceEnd())
                .Select(x => SQSSubscriptionModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}