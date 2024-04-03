using System.Linq;
using Intent.Modelers.AWS.DynamoDB.Api;

namespace Intent.Modules.Modelers.AWS.DynamoDB.Api;

public static class EntityModelExtensions
{
    /// <summary>
    /// Is the <see cref="EntityModel"/> an Aggregate Root? An Aggregate Root is owned by nothing.
    /// </summary>
    public static bool IsAggregateRoot(this EntityModel entity)
    {
        return !entity.AssociationEnds()
            .Any(x => x.IsSourceEnd() && !x.IsCollection && !x.IsNullable);
    }
}