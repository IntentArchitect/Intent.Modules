---
contentHash: BE1F2CA9C151D3C3431F4A177B123FF9CE6A3FC3246CA4ABE69FA135C307EA35
---
# File Builder API Cheatsheet

Quick-reference for common `CSharpFile` fluent calls.  
For full patterns with rules, read the files in `resources/patterns/` before generating code.

## Contents

- [File Builder API Cheatsheet](#file-builder-api-cheatsheet)
- [Contents](#contents)
- [File Setup](#file-setup)
- [Namespaces](#namespaces)
- [Type Declarations](#type-declarations)
- [Members](#members)
- [Model \& Type Integration](#model--type-integration)
- [Advanced Expressions](#advanced-expressions)
- [Build Lifecycle Hooks](#build-lifecycle-hooks)
- [Top-Level Statements](#top-level-statements)
- [Template Contract](#template-contract)
- [Control Flow](#control-flow)
- [Advanced Types](#advanced-types)

===

## File Setup

```csharp
// Always use GetNamespace() and GetFolderPath() — never hardcode strings.
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
```

===

## Namespaces

```csharp
.AddUsing("System")
.AddUsing("Microsoft.Extensions.DependencyInjection")

if (useTopLevelStatements)
{
  CSharpFile.AddUsing(this.GetNamespace());
}

// GetTemplate is a template lookup: call it in a build callback, never in the constructor (Must #7).
// It throws when nothing matches — pass DoNotThrow when the template is optional:
CSharpFile.OnBuild(file =>
{
  var template = GetTemplate<IClassProvider>(templateDependency, TemplateDiscoveryOptions.DoNotThrow);
  if (template != null)
  {
    file.AddUsing(template.Namespace);
  }
});

foreach (var ns in requiredNamespaces)
{
  AddUsing(ns);
}
```

Use `AddUsing(...)` when the namespace is needed independently of a specific type reference, or is discovered dynamically.  
Prefer `UseType("Namespace.Type")` when the namespace should only be introduced if that exact concrete type is required, including method signatures.

> **Builder inference rule**: `UseType(...)` works because the builder tracks the type reference and auto-introduces the namespace. Raw strings are opaque, including return/parameter type strings.

===

## Type Declarations

```csharp
.AddClass("MyClass", @class => { })
.AddInterface("IMyService", @interface => { })
.AddRecord("MyRecord", record => { })
.AddEnum("MyEnum", @enum => { @enum.AddLiteral("Active"); })
```

===

## Members

```csharp
// Constructor with auto-field:
@class.AddConstructor(ctor =>
  ctor.AddParameter("string", "value", p => p.IntroduceReadonlyField()));

// Method:
@class.AddMethod("void", "DoWork", method =>
{
  method.AddParameter("int", "count");
  method.AddStatement("return;");
});

// Interface method (async):
@interface.AddMethod("Task", "Execute", method => method.Async());

// Optional CancellationToken parameter — emits "CancellationToken cancellationToken = default" and its using:
@class.AddMethod("Task", "RunAsync", method => method.Async().AddOptionalCancellationTokenParameter());

// Property:
@class.AddProperty("string", "Name");

// Field (PrivateReadOnly() exists on fields only, not on properties):
@class.AddField("ILogger", "_logger", f => f.PrivateReadOnly());

// Public members at the 3.8.0 floor: Constant(value) emits "public const";
// a public static value is a get-only static property:
@class.AddField("string", "Name", f => f.Constant("\"Customer\""));
@class.AddProperty("string[]", "Attributes", p => p.Static().WithoutSetter().WithInitialValue("[\"Id\", \"Name\"]"));
```

At the 3.8.0 floor a field offers `Private`, `PrivateReadOnly`, `Protected`, `ProtectedReadOnly`, `Constant`, `PrivateConstant` and `Static`. `Public(...)` and `ReadOnly(...)`, the route to a `public static readonly` field, need `Intent.Common.CSharp` 3.9.8 or later: raise the `Intent.Common.CSharp` `<dependency>` (`.imodspec`) and the `Intent.Modules.Common.CSharp` package reference (`.csproj`) to 3.9.8, with `Intent.Modules.Common` / the `Intent.Common` `<dependency>` as `known-build-gotchas`' NU1605 section describes, or use the property form above.

===

## Model & Type Integration

```csharp
// 1) Register type sources early so GetTypeName(...) can resolve model references.
//    Pass the target template's id or role as a string — these two are roles published by
//    Intent's Domain modules (see intent-module-orchestrator for a table of common role values):
AddTypeSource("Domain.Entity.Interface");
AddTypeSource("Domain.Enum");

CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    // Declare the class in the constructor. AddClass's configure lambda is itself deferred to the
    // build phase; an explicit OnBuild just makes it visible where the lookups run.
    .AddClass(Model.Name)
    // Map the model in a build callback, once every template is registered (Must #7):
    .OnBuild(file =>
    {
        var @class = file.Classes.First();

        // 2) Attribute -> Property mapping (model-driven):
        foreach (var attribute in Model.Attributes)
        {
            var propertyType = GetTypeName(attribute);
            @class.AddProperty(propertyType, attribute.Name.ToPascalCase(), prop =>
            {
                prop.RepresentsModel(attribute);

                // Static only when model semantics require it.
                // prop.Static();

                // If the target member API exposes WithOptional(bool), map from model optionality.
                // For CSharpProperty, optionality is represented via resolved type/nullability
                // (and explicit modifiers such as Required()) rather than WithOptional(...).
            });
        }

        // 3) Operation -> Method mapping (model-driven):
        foreach (var operation in Model.Operations)
        {
            @class.AddMethod(GetTypeName(operation), operation.Name.ToPascalCase(), method =>
            {
                method.RepresentsModel(operation);

                foreach (var parameter in operation.Parameters)
                {
                  method.AddParameter(GetTypeName(parameter), parameter.Name.ToParameterName(), p => p.WithDefaultValue(parameter.Value));
                }

                // If the operation is async (your module's own rule — e.g. a stereotype or setting),
                // mark the method async and add "CancellationToken cancellationToken = default":
                if (IsAsync(operation))
                {
                  method.Async();
                  method.AddOptionalCancellationTokenParameter();
                }
            });
        }

        // 4) Constructor DI parameters should become private readonly fields.
        @class.AddConstructor(ctor =>
        ctor.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger<" + Model.Name + ">"), "logger",
        p => p.IntroduceReadonlyField()));
    });

// 5) TemplateId-based type resolution — also in a build callback, never in the constructor.
//    GetTypeName(templateId, model) THROWS when no instance matches, so a "?? throw" after it never runs:
CSharpFile.OnBuild(file =>
{
    var dtoType = GetTypeName(MyDtoTemplate.TemplateId, Model);

    //    When the other template is optional, use TryGetTypeName and branch:
    if (TryGetTypeName(MyDtoTemplate.TemplateId, Model, out var optionalDtoType))
    {
        // use optionalDtoType
    }
});

// 6) Concrete type usage: never hardcode namespace + using separately when UseType can do both.
var cancellationTokenType = UseType("System.Threading.CancellationToken");
```

===

## Advanced Expressions

```csharp
// LAMBDAS: use CSharpLambdaBlock for LINQ and scoped logic. Do not handcraft => strings.
method.AddInvocationStatement("items.Where", where => where
  .AddArgument(new CSharpLambdaBlock("x")
      .WithExpressionBody(new CSharpStatement("x.IsActive"))));

method.AddInvocationStatement("items.Select", select => select
  .AddArgument(new CSharpLambdaBlock("x")
      .WithExpressionBody(new CSharpStatement("x.Name"))));

// OBJECT INITIALIZERS: use CSharpObjectInitializerBlock with AddInitStatement, never raw { ... } strings.
// CSharpVariableDeclaration writes the "var " itself — pass only the name ("var dto" renders "var var dto").
method.AddStatement(new CSharpAssignmentStatement(
  new CSharpVariableDeclaration("dto"),
  new CSharpObjectInitializerBlock("new MyDto")
      .AddInitStatement("Id", "entity.Id")
      .AddInitStatement("Name", "entity.Name")
      .AddInitStatement("IsActive", "entity.IsActive")
      .WithSemicolon()));

// MODERN CHAINING: use AddInvocation + OnNewLine for readable fluent chains.
method.AddStatement(new CSharpStatement("services")
  .AddInvocation("AddOptions")
  .AddInvocation("AddLogging", x => x.OnNewLine())
  .AddInvocation("AddHealthChecks", x => x.OnNewLine()));

// FORBIDDEN / OBSOLETE:
// CSharpMethodChainStatement is [Obsolete] and must not be used.
```

===

## Build Lifecycle Hooks

```csharp
.OnBuild(file =>
{
  // Structural composition. Phase 1: the OnBuild callbacks of EVERY template run first,
  // ordered by priority (lower number runs first; the default is 0).
  file.Classes.First().AddMethod("void", "Generated");
})
.AfterBuild(file =>
{
  // Final reconciliation. Phase 2: starts only after every OnBuild of every template has run,
  // whatever the priorities — this number only orders it against other AfterBuild callbacks.
  file.Classes.First().FindMethod("Generated")?.AddStatement("// reconciled");
}, 1000)  // explicit priority — consistent bands within each phase: 0 / 100 / 500 / 1000
```

> Callbacks run only for templates whose `CanRunTemplate()` is true when the phase starts. A callback registered from inside another callback is queued into the running phase.
> See `resources/patterns/lifecycle-hooks.cs` for priority bands, factory extension patterns, and FindMethod usage.

===

## Top-Level Statements

```csharp
// File-level top-level statements with local methods (in the template constructor).
// The Software Factory builds the file itself — never call a build method by hand.
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
  .AddUsing("System")
  .AddTopLevelStatements(tls =>
  {
      tls.AddStatement("Console.WriteLine(\"Hello world!\");");
      tls.AddLocalMethod("Task", "LocalMethod", localMethod =>
      {
          localMethod.AddParameter("object", "parameter");
          localMethod.Static().Async();
          localMethod.AddStatement("var variable = new object();");
      });
  })
  .AddClass("Class");
```

===

## Template Contract

```csharp
// These three members are the full required contract. Keep them exactly as shown.
[IntentManaged(Mode.Fully)]
public CSharpFile CSharpFile { get; }

[IntentManaged(Mode.Fully)]
protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

[IntentManaged(Mode.Fully)]
public override string TransformText() => CSharpFile.ToString();
```

===

## Control Flow

> Read `resources/patterns/control-flow.cs` before generating any control flow logic.

```csharp
// IF / ELSE — else and else-if are SIBLINGS on the method, not children of the if block:
method.AddIfStatement("x == 0", @if => @if.AddStatement("throw new Exception();"));
method.AddElseIfStatement("x < 0", e => e.AddStatement("x = 0;"));  // sibling
method.AddElseStatement(e => e.AddStatement("Process(x);"));          // sibling

// FOREACH:
method.AddForEachStatement("item", "items", loop => loop.AddStatement("Process(item);"));

// WHILE:
method.AddWhileStatement("!done", loop => loop.AddStatement("done = Next();"));

// USING:
method.AddUsingBlock("var s = GetScope()", b => b.AddStatement("s.Use();"));

// TRY / CATCH / FINALLY — catch and finally are SIBLINGS on the method:
method.AddTryBlock(@try => @try.AddStatement("Risky();"));
method.AddCatchBlock("Exception", "ex", @catch => @catch.AddStatement("Log(ex);"));  // sibling
method.AddFinallyBlock(@finally => @finally.AddStatement("Cleanup();"));              // sibling

// INVOCATION (preferred over obsolete CSharpMethodChainStatement):
method.AddInvocationStatement("service.Run", s => s.AddArgument("request").AddArgument("ct"));

// FLUENT CHAIN — .OnNewLine() forces each call to a new indented line:
method.AddStatement(new CSharpStatement("builder")
  .AddInvocation("StepOne")
  .AddInvocation("StepTwo", s => s.OnNewLine())
  .AddInvocation("StepThree", s => s.OnNewLine()));

// ASSIGNMENT — the right-hand side carries the ";" (without WithSemicolon() none is written):
method.AddAssignmentStatement("var result", new CSharpStatement("await svc.GetAsync()").WithSemicolon());
```

===

## Advanced Types

> Read `resources/patterns/advanced-types.cs` before generating generics, attributes, or nested types.

```csharp
// GENERICS — a method's own type parameter needs its own name (reusing the class's T is CS0693):
@class.AddGenericParameter("T", out var t);
@class.AddGenericTypeConstraint(t, c => c.AddType("class"));
method.AddGenericParameter("TResult", out var tResult).AddGenericTypeConstraint(tResult, c => c.AddType("new()"));

// INHERITANCE:
@class.WithBaseType("Base");
@class.ImplementsInterface("IContract");

// ATTRIBUTES (AddArgument handles both positional and named args):
@class.AddAttribute("MyAttr", a => a.AddArgument("Value").AddArgument("Named = true"));

// XML DOCS:
@class.XmlComments.AddStatements("/// <summary>Summary.</summary>");
method.AddParameter("string", "x", p => p.WithXmlDocComment("The x value."));
@class.InheritsXmlDocComments();  // emits: <inheritdoc />

// MODIFIERS:
@class.Static(); @class.Abstract(); @class.Partial();
method.Virtual(); method.Override(); method.Abstract();

// METADATA — TWO uses. Guard every read: GetMetadata<T> throws on an absent key, returns null
// (no exception) when the value is not a T, and only takes reference types (GetMetadata<bool>
// does not compile). TryGetMetadata<T> checks both and works for any T.
// 1. YOUR OWN cross-step state — set in one callback, read back in a later one:
node.AddMetadata("key", value);              // throws if that key is already present
if (node.TryGetMetadata<bool>("key", out var v) && v) { /* use v */ }

// 2. WHICH DESIGNER ELEMENT a generated member came from — never match on names:
//    a) RepresentsModel(element) records it on the member, read back as RepresentedModel
//       (RepresentedModel needs Intent.Common.CSharp 3.8.5+ — raise the Intent.Common.CSharp dependency (.imodspec)
//       and the Intent.Modules.Common.CSharp package reference (.csproj) to 3.8.5, and Intent.Modules.Common /
//       the Intent.Common dependency to 3.7.3 with them, or restore fails with NU1605; see known-build-gotchas):
if (method.RepresentedModel is OperationModel operation) { /* designer element */ }
//    b) The "model" metadata key is a CONVENTION, not a builder feature: it is there only if the
//       template that built the member called AddMetadata("model", element). Many do; check yours.
if (method.TryGetMetadata<OperationModel>("model", out var op)) { /* op = designer element */ }
// See intent-module-orchestrator § "The Model Bridge".
```
