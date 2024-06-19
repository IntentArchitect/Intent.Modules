using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpDefaultIntentManagedAttribute : CSharpAssemblyAttribute
{
    protected CSharpDefaultIntentManagedAttribute() : base("DefaultIntentManaged")
    {
    }

    public static CSharpDefaultIntentManagedAttribute Ignore()
    {
        return new CSharpDefaultIntentManagedAttribute()
            .AddArgument("Mode.Ignore") as CSharpDefaultIntentManagedAttribute;
    }

    public static CSharpDefaultIntentManagedAttribute Fully()
    {
        return new CSharpDefaultIntentManagedAttribute()
            .AddArgument("Mode.Fully") as CSharpDefaultIntentManagedAttribute;
    }

    public static CSharpDefaultIntentManagedAttribute Merge()
    {
        return new CSharpDefaultIntentManagedAttribute()
            .AddArgument("Mode.Merge") as CSharpDefaultIntentManagedAttribute;
    }

    public static CSharpDefaultIntentManagedAttribute IgnoreBody()
    {
        return Fully().AddArgument("Body = Mode.Ignore") as CSharpDefaultIntentManagedAttribute;
    }

    public CSharpDefaultIntentManagedAttribute WithSignatureFully()
    {
        return WithProperty("Signature", "Mode.Fully");
    }

    public CSharpDefaultIntentManagedAttribute WithSignatureMerge()
    {
        return WithProperty("Signature", "Mode.Merge");
    }

    public CSharpDefaultIntentManagedAttribute WithSignatureIgnore()
    {
        return WithProperty("Signature", "Mode.Ignore");
    }

    public CSharpDefaultIntentManagedAttribute WithBodyIgnored()
    {
        return WithProperty("Body", "Mode.Ignore");
    }

    public CSharpDefaultIntentManagedAttribute WithBodyFully()
    {
        return WithProperty("Body", "Mode.Fully");
    }
    
    public CSharpDefaultIntentManagedAttribute WithBodyMerge()
    {
        return WithProperty("Body", "Mode.Merge");
    }

    public CSharpDefaultIntentManagedAttribute WithAccessModifierNone()
    {
        return WithProperty("AccessModifiers", "AccessModifiers.None");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessModifierPrivate()
    {
        return WithProperty("AccessModifiers", "AccessModifiers.Private");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessModifierProtected()
    {
        return WithProperty("AccessModifiers", "AccessModifiers.Protected");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessModifierInternal()
    {
        return WithProperty("AccessModifiers", "AccessModifiers.Internal");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessModifierPublic()
    {
        return WithProperty("AccessModifiers", "AccessModifiers.Public");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsClass()
    {
        return WithProperty("Targets", "Targets.Classes");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsMethods()
    {
        return WithProperty("Targets", "Targets.Methods");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsConstructors()
    {
        return WithProperty("Targets", "Targets.Constructors");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsFields()
    {
        return WithProperty("Targets", "Targets.Fields");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsProperties()
    {
        return WithProperty("Targets", "Targets.Properties");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsInterfaces()
    {
        return WithProperty("Targets", "Targets.Interfaces");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsEnums()
    {
        return WithProperty("Targets", "Targets.Enums");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsNamespaces()
    {
        return WithProperty("Targets", "Targets.Namespaces");
    }
    
    public CSharpDefaultIntentManagedAttribute WithAccessTargetsUsings()
    {
        return WithProperty("Targets", "Targets.Usings");
    }

    public override string GetText(string indentation)
    {
        return $"{indentation}[assembly: {Name}{(Statements.Any() ? $"({string.Join(", ", Statements)})" : string.Empty)}]";
    }

    private CSharpDefaultIntentManagedAttribute WithProperty(string property, string mode)
    {
        var existing = this.FindStatement(x => x.GetText(string.Empty).StartsWith($"{property} = "));
        if (existing != null)
            existing.Replace(new CSharpStatement($"{property} = {mode}"));
        else
        {
            AddArgument($"{property} = {mode}");
        }

        return this;
    }
}