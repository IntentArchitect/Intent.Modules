---
contentHash: EE5A9373034A2A30D661325586FFD3EE89730E93B9CD5B25E832623A39BB8102
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
- --

## File Setup

```csharp
// Always use GetNamespace() and GetFolderPath() — never hardcode strings.
CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
```

- --

## Namespaces

```csharp
.AddUsing("System")
.AddUsing("Microsoft.Extensions.DependencyInjection")

if (useTopLevelStatements)
{
    CSharpFile.AddUsing(this.GetNamespace());
}

var template = GetTemplate<IClassProvider>(templateDependency);
if (template != null)
{
    AddUsing(template.Namespace);
}

foreach (var ns in requiredNamespaces)
{
    AddUsing(ns);
}
```

Use `AddUsing(...)` when the namespace is needed independently of a specific type reference, or is discovered dynamically.  
Prefer `UseType("Namespace.Type")` when the namespace should only be introduced if that exact concrete type is required, including method signatures.

> **Builder inference rule**: `UseType(...)` works because the builder tracks the type reference and auto-introduces the namespace. Raw strings are opaque, including return/parameter type strings.

- --

## Type Declarations

```csharp
.AddClass("MyClass", @class => { })
.AddInterface("IMyService", @interface => { })
.AddRecord("MyRecord", record => { })
.AddEnum("MyEnum", @enum => { @enum.AddLiteral("Active"); })
```

- --

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

// Property:
@class.AddProperty("string", "Name");
@class.AddProperty("ILogger", "_logger", p => p.PrivateReadOnly());
```

- --

## Model & Type Integration

```csharp
// 1) Register type sources early so GetTypeName(...) can resolve model references.
AddTypeSource(TemplateRoles.Domain.Entity.Interface);
AddTypeSource(TemplateRoles.Domain.Enum);

CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
    .AddClass(Model.Name, @class =>
    {
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
                // For CSharpProperty in this repo, optionality is represented via resolved type/nullability
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
                    method.AddParameter(GetTypeName(parameter), parameter.Name.ToParameterName(),
                        p => p.WithDefaultValue(parameter.Value));
                }

                // If operation behavior is async / returns Task, mark the method async.
                if (operation.IsAsync())
                {
                    method.Async();
                    method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken",
                        p => p.WithDefaultValue("default"));
                }
            });
        }

        // 4) Constructor DI parameters should become private readonly fields.
        @class.AddConstructor(ctor =>
            ctor.AddParameter(UseType("Microsoft.Extensions.Logging.ILogger<" + Model.Name + ">"), "logger",
                p => p.IntroduceReadonlyField()));
    });

// 5) TemplateId-based type resolution:
var dtoType = GetTypeName(MyDtoTemplate.TemplateId, Model)
              ?? throw new InvalidOperationException("DTO template type could not be resolved.");

// 6) Concrete type usage: never hardcode namespace + using separately when UseType can do both.
var cancellationTokenType = UseType("System.Threading.CancellationToken");
```

- --

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
method.AddStatement(new CSharpAssignmentStatement(
    new CSharpVariableDeclaration("var dto"),
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

- --

## Build Lifecycle Hooks

```csharp
.OnBuild(file =>
{
    // Structural composition — runs during build, before AfterBuild.
    // Priority defaults to 0 (lowest number = runs first).
    file.Classes.First().AddMethod("void", "Generated");
})
.AfterBuild(file =>
{
    // Final reconciliation — runs after ALL OnBuild delegates across all templates.
    // Use for cross-template wiring or late enrichment.
    file.Classes.First().FindMethod("Generated")?.AddStatement("// reconciled");
}, 1000)  // explicit priority — use consistent bands: 0 / 100 / 500 / 1000
```

> See `resources/patterns/lifecycle-hooks.cs` for priority bands, factory extension patterns, and FindMethod usage.

- --

## Top-Level Statements

```csharp
// File-level top-level statements with local methods.
var fileBuilder = new CSharpFile("Namespace", "RelativeLocation")
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
    .AddClass("Class")
    .CompleteBuild();
```

- --

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

- --

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

// ASSIGNMENT:
method.AddAssignmentStatement("var result", new CSharpStatement("await svc.GetAsync()"));
```

- --

## Advanced Types

> Read `resources/patterns/advanced-types.cs` before generating generics, attributes, or nested types.

```csharp
// GENERICS:
@class.AddGenericParameter("T", out var t);
method.AddGenericParameter(t).AddGenericTypeConstraint(t, c => c.AddType("class"));

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

// METADATA (guard all reads — never call GetMetadata without HasMetadata / TryGetMetadata):
node.AddMetadata("key", value);
if (node.TryGetMetadata<bool>("key", out var v) && v) { /* use v */ }
```
