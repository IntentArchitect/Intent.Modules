#nullable enable
using System;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpProperty : ICSharpMember<ICSharpProperty>, ICSharpReferenceable
{
    string AccessModifier { get; }
    string OverrideModifier { get; }
    string Type { get; }
    bool IsReadOnly { get; }
    bool IsStatic { get; }
    bool IsRequired { get; }
    string InitialValue { get; }
    string ExplicitlyImplementing { get; }
    bool IsOmittedFromRender { get; }
    ICSharpPropertyAccessor Getter { get; }
    ICSharpPropertyAccessor Setter { get; }
    string GetReferenceName();
    ICSharpProperty Internal();
    ICSharpProperty Private();
    ICSharpProperty Protected();
    ICSharpProperty ProtectedInternal();
    ICSharpProperty Public();
    ICSharpProperty WithoutAccessModifier();
    ICSharpProperty Override();
    ICSharpProperty New();
    ICSharpProperty Virtual();
    ICSharpProperty Static();
    ICSharpProperty Required();
    ICSharpProperty PrivateSetter();
    ICSharpProperty ProtectedSetter();
    ICSharpProperty Init();
    ICSharpProperty ReadOnly();
    ICSharpProperty WithoutSetter();
    ICSharpProperty WithInitialValue(string initialValue);
    ICSharpProperty ExplicitlyImplements(string @interface);
    ICSharpProperty WithBackingField(Action<ICSharpField>? configure = null);
    ICSharpProperty WithInstantiation();
    ICSharpProperty MoveTo(int propertyIndex);
    ICSharpProperty MoveToFirst();
    ICSharpProperty MoveToLast();
}