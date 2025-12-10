#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using Intent.CodeToModelOperations;

namespace Intent.Modules.Common.CSharp.CodeToModelSynchronization;

public interface ISynchronizeCSharpCodeToModel : ISynchronizeCodeToModel
{
    void Accept(ICSharpSemanticComparisonNode rootComparisonNode);
}