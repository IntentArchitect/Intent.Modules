using System.Linq;

namespace Intent.Modelers.Domain.Api;

public static class ClassModelAssociationExtensions
{
    /// <summary>
    /// Is the <see cref="ClassModel"/> an Aggregate Root?
    /// An Aggregate Root is owned by nothing and can be instantiated.
    /// </summary>
    public static bool IsAggregateRoot(this ClassModel classModel)
    {
        var owningAssociations = classModel.AssociationEnds()
            .Where(x => x.IsSourceEnd() && !x.IsCollection && !x.IsNullable)
            .ToArray();
        return !owningAssociations.Any() && !classModel.IsAbstract;
    }
}