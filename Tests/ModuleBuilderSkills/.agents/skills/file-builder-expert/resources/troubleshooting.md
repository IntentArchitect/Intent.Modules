---
contentHash: 0620B91CEE87D3F2DADAD22D333D4B89BFF9B7557FB6DCE73F6564C6B66E6317
---
# File Builder Troubleshooting

Indexed failure modes for CSharpFile-based templates.

===

## 1. `ToString` Before Build Completion

- *Symptom:** `Build() needs to be called before ToString()`  
- *Cause:** `TransformText()` was called before the builder lifecycle ran. Usually means `ICSharpFileBuilderTemplate` is not implemented, so the framework never triggers `Build()`.  
- *Fix:** Implement `ICSharpFileBuilderTemplate` on the template class. Keep `TransformText` as `return CSharpFile.ToString();` only.

===

## 2. Empty Structural Output

- *Symptom:** Exception: `No type or top-level statements were specified`  
- *Cause:** `CSharpFile` was constructed but no class, interface, record, enum, or top-level statements were added before `Build()` ran.  
- *Fix:** Add at least one structural declaration (e.g. `.AddClass(...)`) in the constructor or in an `OnBuild` callback.

===

## 3. Invalid `OnBuild` Timing

- *Symptom:** `This file has already been built`  
- *Cause:** `OnBuild(...)` was called after the build lifecycle had already completed — for example, inside an `AfterBuild` callback or in a post-construction hook.  
- *Fix:** Register all `OnBuild` callbacks during constructor setup. Never queue new `OnBuild` callbacks from within an `AfterBuild` handler.

===

## 4. Invalid `AfterBuild` Timing

- *Symptom:** `The AfterBuild step has already been run for this file`  
- *Cause:** `AfterBuild(...)` was registered after the lifecycle already completed the `AfterBuild` phase.  
- *Fix:** Register all `AfterBuild` callbacks during constructor setup or from within an `OnBuild` callback that runs while the phase is still open.

===

## 5. Pending Configuration Delegates

- *Symptom:** `Pending configurations have not been executed`  
- *Cause:** Build lifecycle was interrupted, or callbacks were collected into a queue but never flushed. Can happen when the `CSharpFile` constructor lambda throws before completing.  
- *Fix:** Ensure constructor lambdas are deterministic and do not throw. Avoid conditional queue mutations inside `AddClass` / `AddMethod` lambdas where an exception would leave the builder in a half-configured state.

===

## 6. Metadata-Resolution Failures

- *Symptom:** `KeyNotFoundException`, `InvalidCastException`, or `NullReferenceException` during post-processing  
- *Cause:** Calling `GetMetadata<T>(key)` when the key was never set, or casting to the wrong type.  
- *Fix:** Always guard with `HasMetadata` or `TryGetMetadata<T>`. Do not assume metadata set by one template is always present when consumed by another (different execution orders, optional features).

```csharp
// Unsafe:
var flag = method.GetMetadata<bool>("my-key");  // throws if key absent

// Safe:
if (method.TryGetMetadata<bool>("my-key", out var flag) && flag)
{
    // use flag
}
```

===

## 7. Mismatched `TemplateId`

- *Symptom:** Template not discovered, or registration resolves to wrong implementation  
- *Cause:** `TemplateId` constant in the template class and the value passed to the registration class differ (case-sensitive).  
- *Fix:** Define `TemplateId` as a `public const string` in the template and reference it by name in the registration:

```csharp
// Template:
public const string TemplateId = "My.Module.MyTemplate";

// Registration:
public class MyTemplateRegistration : SingleFileTemplateRegistration<MyTemplate>
{
    public override string TemplateId => MyTemplate.TemplateId;  // reference the constant
}
```

===

## 8. Wrong Registration Type

- *Symptom:** Template runs once but model-specific files are not generated (or vice versa)  
- *Cause:** Using `SingleFileTemplateRegistration` when the template should produce one file per model element.  
- *Fix:**

| Scenario | Registration base |
|----------|-------------------|
| One output file | `SingleFileTemplateRegistration` |
| One file per model element | `FilePerModelTemplateRegistration<TModel>` — must also override `GetModels` |
| Event/pipeline-driven | `ITemplateRegistration` directly |

===

## 9. Usings don't follow members relocated to another file

- *Principle:** usings belong to the **file that emits the member**, and the builder only adds them for type references it can *track* — i.e. ones resolved through the type system (`UseType` / `GetTypeName`) against **that file's** template. So whenever generated members are emitted into a file other than the current template's own (code-behind, an aggregating/partial file, a sibling template, a file authored via `OnBuild`/`FindClass` on another template), the relevant usings only follow if you resolve types against the *destination* template.
- *Symptom:** A relocated/extracted member fails to compile — types or attributes (`Task`, `[SupplyParameterFromForm]`, `[Required]`, `IEnumerable<>`, …) not found — even though the same code compiled in its original file.
- *Cause:** (a) members were added with **raw type strings** the builder can't track; and/or (b) the destination file doesn't inherit the source's implicit imports (e.g. a plain `.cs` gets none of Razor's `_Imports`; global usings differ per project); and/or (c) the type was resolved against the **wrong** template, so the using landed on the source file, not the destination.
- *Fix:** Resolve every type/return/attribute through the **destination block's** template — `targetBlock.Template.UseType("Namespace.Type")` (e.g. `code.Template.UseType(...)` where `code` is the destination class) — so the using lands on the file that holds the members. Add `.RemoveSuffix("Attribute")` for attribute names. Ensure the destination template exposes the right context, e.g. `public override ICSharpCodeContext RootCodeContext => CSharpFile.Classes.Single();`. Types referenced only inside **raw statement/expression strings** are never tracked — interpolate a `UseType(...)` into the string, or add the namespace explicitly with `CSharpFile.AddUsing(...)`. See *Split-file / code-behind usings* in `SKILL.md`.

===

## 10. `FindMethod` Returns Only the First Overload

- *Symptom:** When a class has multiple overloaded methods with the same name (e.g. two `HandleAsync` methods for different event types), only the first overload is modified or enriched — subsequent overloads are silently ignored.  
- *Cause:** `cls.FindMethod("HandleAsync")` stops at the first match.  
- *Fix:** Use LINQ to enumerate all matching methods:

```csharp
// Wrong — only finds the first overload
var method = cls.FindMethod("HandleAsync");

// Correct — handles all overloads
var methods = cls.Methods.Where(m => m.Name == "HandleAsync");
foreach (var method in methods)
{
    // apply changes to each overload
}
```

===

## 11. Double semicolon from `AddReturn` wrapping a statement-type expression

- *Symptom:** Generated returns emit a stray second semicolon, e.g. `return await Task.FromResult(value); ;`.
- *Cause:** `method.AddReturn(stmt)` already appends the `;`. `CSharpObjectInitializerBlock` renders as an expression (no own semicolon), so `AddReturn` works cleanly with it — but `CSharpInvocationStatement` (and other statement-type nodes) default to rendering **with** their own trailing `;`, which then doubles up.
- *Fix:** Call `.WithoutSemicolon()` on the inner statement before handing it to `AddReturn` (or to `.AddArgument(...)` / any expression position):

```csharp
// Wrong — invocation keeps its own ';', AddReturn adds another → "...); ;"
method.AddReturn(new CSharpInvocationStatement("await Task.FromResult").AddArgument(value));

// Right — invocation renders as an expression; AddReturn supplies the single ';'
method.AddReturn(new CSharpInvocationStatement("await Task.FromResult").AddArgument(value).WithoutSemicolon());
```

Rule of thumb: any `CSharp*Statement` placed in an expression slot (return value, argument, init value) needs `.WithoutSemicolon()`; object-initializer blocks already behave as expressions.
