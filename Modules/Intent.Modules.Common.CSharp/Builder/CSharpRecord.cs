using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Builder;

[FixFor_Version4("Refactor this and CSharpClass to derive from a base type with common functionality")]
public class CSharpRecord : CSharpClass
{
    public CSharpRecord(string name) : base(name, Type.Record)
    {
    }
}