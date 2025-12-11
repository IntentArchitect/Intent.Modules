#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Intent.Modules.Common.CSharp.CodeToModelSynchronization;

public enum CSharpDifferenceType
{
    Unchanged = 1,
    Added = 2,
    Removed = 3,
    Changed = 4
}