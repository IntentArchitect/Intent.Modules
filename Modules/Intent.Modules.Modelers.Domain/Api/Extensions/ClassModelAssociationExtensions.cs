using System.Linq;

namespace Intent.Modelers.Domain.Api;

public static class ClassModelAssociationExtensions
{
    public static bool IsAggregateRoot(this ClassModel classModel)
    {
        var owningAssociations = classModel.AssociationEnds()
            .Where(x => x.IsSourceEnd() && !x.IsCollection && !x.IsNullable)
            .ToArray();
        return !owningAssociations.Any();
    }
}