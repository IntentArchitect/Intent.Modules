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
- **Class method modifiers are flags that clear only what C# forbids alongside them** (3.10.12) —
  `CSharpClassMethod` replaced its single `OverrideModifier` slot (where every call overwrote the last) with
  `static` / `new` / `virtual` / `abstract` / `sealed` / `override` flags. Each setter keeps compatible flags and
  clears conflicting ones, so conflicting calls stay last-call-wins — exactly the old output — and only
  compatible pairs (`abstract override`, `new virtual`, `new abstract`, `static new`, `sealed override`) render
  differently. A survey of this repository and Intent.Modules.NET found no template calling a compatible pair on
  one method, so no existing output changed. Rejected: making every modifier additive (would turn existing
  last-call-wins sequences such as `Virtual().Override()` into invalid C#). `Sealed()` also applies `override`,
  because C# rejects `sealed` without it. `OverrideModifier` remains as a computed, rendering-order string.
  `Sealed()` / `IsSealed` are declared on the shared `ICSharpMethodDeclaration<T>` beside `Override()`,
  `Virtual()` and `Abstract()` (the user's call, for consistency), not on the class-only interface; so
  `CSharpInterfaceMethod.Sealed()` is a no-op, matching its existing `Virtual()` / `Abstract()`.

- **Collection items and switch-case statements get a `Parent`, as invocation arguments always had** (3.10.12).
  Without one, `Remove`/`Replace`/`InsertAbove`/`InsertBelow` threw "unknown parent", and `AddInvocation` on an
  item silently built a statement nobody rendered. Rejected: setting a parent on a lambda's expression body as
  well - a parented collection there takes its semicolon decision from the lambda, which is a code block, and
  renders a stray `;`.
- **`AddInvocation` drops the default `;` only when it replaces a collection item.** Giving items a parent made
  `AddInvocation` on an item replace it, and an invocation carries `;` by default - `[a.B();]`. The fix is
  confined to that path, which could not run before, so no existing output changes; the caller's `configure`
  can still add one. Rejected, for blast radius: dropping a trailing `;` from every item when a collection
  renders (changes output for any caller that added an invocation as an item), and dropping it whenever the
  parent is not a code block (`CSharpSwitchCodeBlock` is not one either, and its statements need theirs).
- **Statement search does NOT look inside `CSharpAssignmentStatement.Rhs`, deliberately.** It was changed to and
  then reverted the same day: Intent.Modules.NET builds ~60 assignments with rich right-hand sides and calls
  `FindStatement` ~190 times, and searching the Rhs would change which statement an existing predicate finds
  first. Reach an assigned collection's items in two steps instead - find the assignment, then search its
  `Rhs` - which works now that items know their parent. Adding the Rhs to `Statements` was rejected too:
  `GetText` renders the `Rhs` field, so edits made through the list would not render.

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
- **Method modifier rendering order is fixed by the compiler, not style.** `CSharpClassMethod` renders access,
  `static`, `new`, `virtual`/`abstract`, `sealed`, `override`, then `partial`, then `async`. `partial` must follow the
  other member modifiers (`partial static` is CS0267, `partial override` is CS1585); `partial async` is accepted,
  so it stays in that order to keep existing output stable. Verified by compiling the
  `MethodModifierTests.ValidModifierCombinationsTest` snapshot.
