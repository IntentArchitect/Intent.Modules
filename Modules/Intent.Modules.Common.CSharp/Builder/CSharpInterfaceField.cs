namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceField : CSharpField, ICSharpInterfaceField
{
    public CSharpInterfaceField(string type, string name, ICSharpCodeContext @class) : base(type, name, @class)
    {
    }
}