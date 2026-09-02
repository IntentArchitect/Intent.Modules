// PATTERN: Advanced Types — Generics, Inheritance, Attributes, XML Docs, Nested Types, Metadata
// ================================================================================================
// KEY RULES:
//   - Never emit raw "<T>" type parameters as string text. Use AddGenericParameter.
//   - Never emit ": BaseClass" as raw text. Use WithBaseType / ImplementsInterface.
//   - Attributes use AddArgument for BOTH positional and named args (e.g. "AllowMultiple = false").
//   - Metadata: method is AddMetadata (not WithMetadata). Always guard reads with HasMetadata / TryGetMetadata.

// --- GENERICS ---

// Capture the parameter via 'out var' so you can reuse it on constraints later:
@class.AddGenericParameter("T", out var t);

@class.AddMethod("void", "Process", method => method
    .AddGenericParameter(t)                                  // reuse the captured parameter
    .AddGenericParameter("U", out var u)
    .AddGenericTypeConstraint(u, c => c.AddType("class")));  // U : class

// --- INHERITANCE & INTERFACES ---

@class.WithBaseType("BaseType");                             // : BaseType
@class.WithBaseType("GenericBase", new[] { "T", "string" }); // : GenericBase<T, string>
@class.ImplementsInterface("IMyContract");                   // , IMyContract
@class.ImplementsInterfaces(new[] { "IDisposable", "IAsyncDisposable" });

// --- ATTRIBUTES ---

// Positional arguments:
@class.AddAttribute("Serializable");

// Resolve the real CLR attribute type, then trim the suffix for idiomatic emitted output:
@class.AddProperty("string", "Email", property =>
    property.AddAttribute(UseType("System.ComponentModel.DataAnnotations.RequiredAttribute").RemoveSuffix("Attribute")));

// Mix of positional and named arguments (identical AddArgument call for both):
@class.AddAttribute("AttributeUsage", attr => attr
    .AddArgument("AttributeTargets.Class")   // positional
    .AddArgument("AllowMultiple = false")    // named
    .AddArgument("Inherited = true"));       // named

method.AddAttribute("SuppressMessage", attr => attr
    .AddArgument("\"Category\"")
    .AddArgument("\"RuleId\"")
    .AddArgument("Justification = \"Generated code\""));

// Assembly-level attribute (goes at file scope):
file.AddAssemblyAttribute("GeneratedCode", attr => attr
    .AddArgument("\"Intent.Modules\"")
    .AddArgument("\"1.0\""));

// --- XML DOCUMENTATION ---

@class.XmlComments.AddStatements("/// <summary>My class summary.</summary>");

@class.AddMethod("void", "Run", method =>
{
    method.XmlComments.AddStatements("/// <summary>Executes the workflow.</summary>");
    method.AddParameter("string", "name", param =>
    {
        param.WithXmlDocComment("Name of the item.");  // adds <param> tag
    });
});

@class.InheritsXmlDocComments();  // emits <inheritdoc />

// --- MODIFIERS ---

@class.Static();
@class.Abstract();
@class.Partial();

@class.AddMethod("void", "TemplateMethod", method => method.Abstract());  // abstract void
@class.AddMethod("void", "OptionalOverride", method => method.Virtual()); // virtual void
@class.AddMethod("void", "Override", method => method.Override());        // override void

// --- NESTED TYPES ---

@class.AddNestedClass("Inner", nested =>
{
    nested.AddMethod("void", "Handle", method =>
    {
        // Use CSharpInvocationStatement, NOT CSharpMethodChainStatement (obsolete):
        method.AddInvocationStatement("Host.CreateDefaultBuilder", stmt => stmt
            .AddInvocation("ConfigureServices", inv => inv
                .AddArgument(new CSharpLambdaBlock("services")
                    .WithExpressionBody(new CSharpStatement("services")
                        .AddInvocation("AddOptions")))));
    });
});

// Record with a primary constructor:
@class.AddNestedRecord("State", nestedRecord =>
{
    nestedRecord.AddPrimaryConstructor(ctor =>
    {
        ctor.AddParameter("int", "Id");
        ctor.AddParameter("string", "Name");
    });
});

// Enum (nested or top-level):
file.AddEnum("Status", @enum =>
{
    @enum.AddLiteral("Active");
    @enum.AddLiteral("Inactive");
});

// --- METADATA (cross-step state) ---
// Use AddMetadata to tag any builder node with arbitrary data for later phases.
// Always guard reads — never call GetMetadata without first checking HasMetadata or using TryGetMetadata.

method.AddMetadata("telemetry-config", true);

// Safe read with HasMetadata guard:
if (method.HasMetadata("telemetry-config"))
{
    var flag = method.GetMetadata<bool>("telemetry-config");
}

// Preferred: TryGetMetadata avoids double-lookup:
if (method.TryGetMetadata<bool>("telemetry-config", out var enabled) && enabled)
{
    method.AddStatement("// metadata-driven statement");
}
