# Context: Intent.Common.CSharp

## Purpose
Common library support for templates that generate C# — chiefly the fluent `CSharpFile` builder
(`Builder/`), type resolution, and C# style settings. It is consumed as a code library by almost every
C# module, so its public API is a contract with modules this repository cannot see.

## Architectural Decisions
- **Type declarations opt out of an access modifier with `WithoutAccessModifier()`** (3.10.12) — on
  `CSharpClass` (so records too), `CSharpInterface` and `CSharpEnum`, plus `ICSharpClass` / `ICSharpInterface`.
  The name mirrors the member-level `WithoutAccessModifier()` that `CSharpProperty`, `CSharpClassMethod` and
  `CSharpInterfaceMethod` already had, so template authors find one spelling for one concept. Rejected: an
  accessibility enum/setter, which would duplicate the existing `Public()`/`Internal()`/... fluent set; and
  changing the default, which would rewrite every existing consumer's output. `ICSharpEnum` was deliberately
  not given the method because it exposes none of the access-modifier methods.
- **`required` is omitted, not rejected, below C# 11** (3.10.12) — `CSharpProperty` / `CSharpField` render the
  keyword only when `Helpers.SupportsRequiredMembers` says the target project's language version is 11+;
  `IsRequired` still records what the template asked for. This matches how `WithInstantiation` already adapts
  collection initialisers to the language version. The user chose omission over the alternatives: throwing a
  `FriendlyException` (stops generation for projects that merely use a lower `LangVersion`) and always emitting
  (generates code that does not compile). Trade-off accepted: below C# 11 the "must be set" guarantee is lost.

## Invariants & Constraints
- **Type declarations default to `public`.** `AccessModifier` starts as `"public "` on `CSharpClass`,
  `CSharpInterface` and `CSharpEnum`; templates across the module ecosystem rely on that without calling
  `Public()`. Opting out must stay explicit.
- **An empty `AccessModifier` means "no modifier".** Ordering in `CSharpFile` and `CSharpClass.Declarations`
  looks each declaration up in `ICSharpStyleSettings.ElementOrder`, which already has a `""` slot (between
  `protected` and `private readonly`), so modifier-less declarations sort deterministically — keep that slot if
  the order list changes.
- **Adding a member to an `ICSharp*` interface is safe only while the builder's own classes and the internal
  `InterfaceWrappers` are the sole implementers.** That held across this repository and Intent.Modules.NET as
  of 3.10.12; re-check before extending the interfaces again.
- **A `CSharpFile` with no template assumes the latest language features.** Language-version checks in the
  builder must treat "no template / no output target" as supported, so builder-only use (including most of the
  snapshot tests) keeps its existing output.
- **Testing language-version behaviour means mocking `IOutputTarget.GetTargetPath()`.** It is an instance method
  on the SDK interface (not an extension), and `GetProject()` builds the `CSharpProject` from its `[0]` entry;
  left unmocked, NSubstitute returns an empty-metadata target. See `RequiredMemberTests`.
