using System.Linq;
using Intent.Modules.Common.FileBuilders;

namespace Intent.Modules.Common.Tests.FileBuilders.DataFileBuilder;

internal static class DataFileExtensions
{
    public static void Build(this IFileBuilderBase fileBuilder)
    {
        foreach (var item in fileBuilder.GetConfigurationDelegates().OrderBy(x => x.Order))
        {
            item.Invoke();
        }
    }
}