namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpIntentTagModeAttribute : CSharpAssemblyAttribute
{
    protected CSharpIntentTagModeAttribute(string tagName) : base(tagName)
    {
    }
    
    public static CSharpIntentTagModeAttribute Explicit()
    {
        return new CSharpIntentTagModeAttribute("IntentTagModeExplicit");
    }

    public static CSharpIntentTagModeAttribute Implicit()
    {
        return new CSharpIntentTagModeAttribute("IntentTagModeImplicit");
    }
}