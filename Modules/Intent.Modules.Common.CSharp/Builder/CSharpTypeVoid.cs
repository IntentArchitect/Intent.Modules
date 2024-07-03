#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591
public class CSharpTypeVoid : CSharpType
{
    public static readonly CSharpTypeVoid DefaultInstance = new();
    
    public override string ToString()
    {
        return "void";
    }

    public override bool Equals(object? obj)
    {
        return obj is CSharpTypeVoid;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}