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
        return AddArgument("Signature = Mode.Fully") as CSharpIntentManagedAttribute;
    }

    public CSharpIntentManagedAttribute WithBodyIgnored()
    {
        return WithBodySetTo("Mode.Ignore");
    }

    public CSharpIntentManagedAttribute WithBodyFully()
    {
        return WithBodySetTo("Mode.Fully");
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}[{Name}{(Statements.Any() ? $"({string.Join(", ", Statements)})" : string.Empty)}]";
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