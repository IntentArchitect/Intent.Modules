#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public interface ICSharpType
{
    ICSharpType? GetTaskGenericType();
    ICSharpType? GetValueTaskGenericType();
    bool IsTask();
    bool IsValueTask();
    ICSharpType WrapInTask(ICSharpTemplate template);
    ICSharpType WrapInValueTask(ICSharpTemplate template);
    bool IsCollectionType();
    ICSharpType GetCollectionImplementationType();
}