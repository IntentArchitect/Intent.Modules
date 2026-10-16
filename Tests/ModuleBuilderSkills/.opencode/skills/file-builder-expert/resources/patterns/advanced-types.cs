// PATTERN: Advanced Types — Generics, Inheritance, Attributes, XML Docs, Nested Types, Metadata
// ================================================================================================
// KEY RULES:
//   - Never emit raw "<T>" type parameters as string text. Use AddGenericParameter.
//   - Never emit ": BaseClass" as raw text. Use WithBaseType / ImplementsInterface.
//   - Attributes use AddArgument for BOTH positional and named args (e.g. "AllowMultiple = false").
//   - Metadata: method is AddMetadata (not WithMetadata). Always guard reads with HasMetadata / TryGetMetadata.
//   - A method's own generic parameter needs its own name — reusing the class's "T" is CS0693.

// --- GENERICS ---

// Capture the parameter via 'out var' so you can reuse it on constraints later:
@class.AddGenericParameter("T", out var t);
@class.AddGenericTypeConstraint(t, c => c.AddType("class"));  // where T : class

@class.AddMethod("void", "Process", method => method
    .AddGenericParameter("U", out var u)                      // the method's own parameter: a new name, not T
    .AddGenericTypeConstraint(u, c => c.AddType("new()"))     // where U : new()
    .AddParameter("T", "item"));                              // the class's T is in scope as a type

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
                        .AddInvocation("AddOptions")
                        .WithoutSemicolon()))));            // expression body: no ';' inside the lambda
    });
});

// Nested record with a primary constructor. AddNestedRecord needs Intent.Common.CSharp 3.9.6 or later —
// on a lower floor, raise the .imodspec <dependency id="Intent.Common.CSharp"> and the .csproj package reference,
// and raise Intent.Modules.Common / the Intent.Common dependency to 3.9.1 with them (3.9.6 depends on it; NU1605 otherwise),
// plus the Intent.SoftwareFactory.SDK package reference to 3.10.0, which Common 3.9.1 depends on (Intent Architect 4.5.15+):
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
// Always guard reads. GetMetadata<T> throws on an absent key, returns null (no exception) when the
// value is not a T, and only accepts reference types — GetMetadata<bool> does not compile.

method.AddMetadata("telemetry-config", true);

// Value types: TryGetMetadata is the only typed read (and the preferred one for any T):
if (method.TryGetMetadata<bool>("telemetry-config", out var enabled) && enabled)
{
    method.AddStatement("// metadata-driven statement");
}

// Reference types: GetMetadata behind a HasMetadata guard also works:
method.AddMetadata("telemetry-options", new TelemetryOptions());
if (method.HasMetadata("telemetry-options"))
{
    var options = method.GetMetadata<TelemetryOptions>("telemetry-options");  // null if it is another type
}
