#nullable enable
using Intent.Metadata.Models;
using System;

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
    ICSharpProperty Protected();
    ICSharpProperty Private();
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
    ICSharpProperty WithInstantiation(ITypeReference model);
    ICSharpProperty MoveTo(int propertyIndex);
    ICSharpProperty MoveToFirst();
    ICSharpProperty MoveToLast();
}