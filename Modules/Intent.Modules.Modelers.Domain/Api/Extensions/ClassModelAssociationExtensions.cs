using System.Linq;

namespace Intent.Modelers.Domain.Api;

public static class ClassModelAssociationExtensions
{
    public static bool IsAggregateRoot(this ClassModel classModel)
    {
        var otherSources = classModel.AssociationEnds()
            .Where(p => p.IsSourceEnd())
            .ToArray();
        return !otherSources.Any() || otherSources.All(x => x.IsCollection || x.IsNullable);
    }
}