using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Builder;

#nullable enable

[FixFor_Version4("Refactor this and CSharpClass to derive from a base type with common functionality")]
public class CSharpRecord : CSharpClass
{
    public CSharpRecord(string name, CSharpFile parent) : base(name, Type.Record, parent)
    {
    }
    
    public CSharpRecord(string name, ICSharpFile? file, ICSharpCodeContext? parent) : base(name, Type.Record, file, parent)
    {
    }
}