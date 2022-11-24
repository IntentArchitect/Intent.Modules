using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Serverless.AWS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    public static class UsesTableModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<UsesTableTargetEndModel> Table(this LambdaFunctionModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UsesTableModel.SpecializationType && x.IsTargetEnd())
                .Select(x => UsesTableModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<UsesTableSourceEndModel> Lambda(this DynamoDBTableModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == UsesTableModel.SpecializationType && x.IsSourceEnd())
                .Select(x => UsesTableModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}