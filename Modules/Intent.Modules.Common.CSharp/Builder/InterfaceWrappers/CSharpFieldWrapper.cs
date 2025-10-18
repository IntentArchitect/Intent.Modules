#nullable enable

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpFieldWrapper(CSharpField wrapped) :
    CSharpMemberWrapper<CSharpField, ICSharpField>(wrapped), ICSharpField
{
    string ICSharpField.Name => wrapped.Name;
    ICSharpField ICSharpField.ProtectedInternal(string? value) => wrapped.ProtectedInternal(value);

    ICSharpField ICSharpField.ProtectedReadOnly() => wrapped.ProtectedReadOnly();

    ICSharpField ICSharpField.Public(string? value) => wrapped.Public(value);

    ICSharpField ICSharpField.ReadOnly(bool readOnly) => wrapped.ReadOnly(readOnly);

    ICSharpField ICSharpField.ReadOnly(string value, bool readOnly) => wrapped.ReadOnly(value, readOnly);

    ICSharpField ICSharpField.Protected() => wrapped.Protected();

    ICSharpField ICSharpField.Protected(string value) => wrapped.Protected(value);

    ICSharpField ICSharpField.PrivateReadOnly() => wrapped.PrivateReadOnly();

    ICSharpField ICSharpField.Internal(string? value) => wrapped.Internal(value);

    ICSharpField ICSharpField.Private() => wrapped.Private();

    ICSharpField ICSharpField.Private(string value) => wrapped.Private(value);

    ICSharpField ICSharpField.Constant(bool isConstant) => wrapped.Constant(isConstant);

    ICSharpField ICSharpField.Constant(string value) => wrapped.Constant(value);

    ICSharpField ICSharpField.Constant(string value, bool isConstant) => wrapped.Constant(value, isConstant);

    ICSharpField ICSharpField.PrivateConstant(string value) => wrapped.PrivateConstant(value);

    ICSharpField ICSharpField.Static() => wrapped.Static();

    ICSharpField ICSharpField.Required() => wrapped.Required();

    ICSharpField ICSharpField.CanBeNull() => wrapped.CanBeNull();

    ICSharpField ICSharpField.WithAssignment(ICSharpStatement value) => wrapped.WithAssignment((CSharpStatement)value);

    ICSharpField ICSharpField.WithInstantiation() => wrapped.WithInstantiation();
}