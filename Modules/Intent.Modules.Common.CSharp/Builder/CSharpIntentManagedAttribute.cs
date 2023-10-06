using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpIntentManagedAttribute : CSharpAttribute
{
    protected CSharpIntentManagedAttribute() : base("IntentManaged")
    {
    }

    public static CSharpIntentManagedAttribute Ignore()
    {
        return new CSharpIntentManagedAttribute()
            .AddArgument("Mode.Ignore") as CSharpIntentManagedAttribute;
    }

    public static CSharpIntentManagedAttribute Fully()
    {
        return new CSharpIntentManagedAttribute()
            .AddArgument("Mode.Fully") as CSharpIntentManagedAttribute;
    }

    public static CSharpIntentManagedAttribute Merge()
    {
        return new CSharpIntentManagedAttribute()
            .AddArgument("Mode.Merge") as CSharpIntentManagedAttribute;
    }

    public static CSharpIntentManagedAttribute IgnoreBody()
    {
        return Fully().AddArgument("Body = Mode.Ignore") as CSharpIntentManagedAttribute;
    }

    public CSharpIntentManagedAttribute WithSignatureFully()
    {
        return WithSignatureSetTo("Mode.Fully");
    }

    public CSharpIntentManagedAttribute WithSignatureMerge()
    {
        return WithSignatureSetTo("Mode.Merge");
    }

    public CSharpIntentManagedAttribute WithSignatureIgnore()
    {
        return WithSignatureSetTo("Mode.Ignore");
    }

    public CSharpIntentManagedAttribute WithBodyIgnored()
    {
        return WithBodySetTo("Mode.Ignore");
    }

    public CSharpIntentManagedAttribute WithBodyFully()
    {
        return WithBodySetTo("Mode.Fully");
    }
    
    public CSharpIntentManagedAttribute WithBodyMerge()
    {
        return WithBodySetTo("Mode.Merge");
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}[{Name}{(Statements.Any() ? $"({string.Join(", ", Statements)})" : string.Empty)}]";
    }

    private CSharpIntentManagedAttribute WithSignatureSetTo(string mode)
    {
        var existing = this.FindStatement(x => x.GetText(string.Empty).StartsWith("Signature = "));
        if (existing != null)
            existing.Replace($"Signature = {mode}");
        else
        {
            AddArgument($"Signature = {mode}");
        }

        return this;
    }

    private CSharpIntentManagedAttribute WithBodySetTo(string mode)
    {
        var existing = this.FindStatement(x => x.GetText(string.Empty).StartsWith("Body = "));
        if (existing != null)
            existing.Replace($"Body = {mode}");
        else
        {
            AddArgument($"Body = {mode}");
        }

        return this;
    }
}