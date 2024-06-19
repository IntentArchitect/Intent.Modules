using System;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpPropertyWrapper(CSharpProperty wrapped) :
    CSharpMemberWrapper<CSharpProperty, ICSharpProperty>(wrapped), ICSharpProperty
{
    string ICSharpProperty.AccessModifier => wrapped.AccessModifier;

    string ICSharpProperty.OverrideModifier => wrapped.OverrideModifier;

    string ICSharpProperty.Type => wrapped.Type;

    bool ICSharpProperty.IsReadOnly => wrapped.IsReadOnly;

    bool ICSharpProperty.IsStatic => wrapped.IsStatic;

    bool ICSharpProperty.IsRequired => wrapped.IsRequired;

    string ICSharpProperty.InitialValue => wrapped.InitialValue;

    string ICSharpProperty.ExplicitlyImplementing => wrapped.ExplicitlyImplementing;

    bool ICSharpProperty.IsOmittedFromRender => wrapped.IsOmittedFromRender;

    ICSharpPropertyAccessor ICSharpProperty.Getter => wrapped.Getter;

    ICSharpPropertyAccessor ICSharpProperty.Setter => wrapped.Setter;

    string ICSharpProperty.GetReferenceName() => wrapped.GetReferenceName();

    ICSharpProperty ICSharpProperty.Protected() => wrapped.Protected();

    ICSharpProperty ICSharpProperty.Private() => wrapped.Private();

    ICSharpProperty ICSharpProperty.WithoutAccessModifier() => wrapped.WithoutAccessModifier();

    ICSharpProperty ICSharpProperty.Override() => wrapped.Override();

    ICSharpProperty ICSharpProperty.New() => wrapped.New();

    ICSharpProperty ICSharpProperty.Virtual() => wrapped.Virtual();

    ICSharpProperty ICSharpProperty.Static() => wrapped.Static();

    ICSharpProperty ICSharpProperty.Required() => wrapped.Required();

    ICSharpProperty ICSharpProperty.PrivateSetter() => wrapped.PrivateSetter();

    ICSharpProperty ICSharpProperty.ProtectedSetter() => wrapped.ProtectedSetter();

    ICSharpProperty ICSharpProperty.Init() => wrapped.Init();

    ICSharpProperty ICSharpProperty.ReadOnly() => wrapped.ReadOnly();

    ICSharpProperty ICSharpProperty.WithoutSetter() => wrapped.WithoutSetter();

    ICSharpProperty ICSharpProperty.WithInitialValue(string initialValue) => wrapped.WithInitialValue(initialValue);

    ICSharpProperty ICSharpProperty.ExplicitlyImplements(string @interface) => wrapped.ExplicitlyImplements(@interface);

    ICSharpProperty ICSharpProperty.WithBackingField(Action<ICSharpField> configure) => wrapped.WithBackingField(configure);

    ICSharpProperty ICSharpProperty.MoveTo(int propertyIndex) => wrapped.MoveTo(propertyIndex);

    ICSharpProperty ICSharpProperty.MoveToFirst() => wrapped.MoveToFirst();

    ICSharpProperty ICSharpProperty.MoveToLast() => wrapped.MoveToLast();

    string IHasCSharpName.Name => wrapped.Name;
}