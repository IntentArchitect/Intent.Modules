using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder.InterfaceWrappers;

internal class CSharpFieldWrapper(CSharpField wrapped) :
    CSharpMemberWrapper<CSharpField, ICSharpField>(wrapped), ICSharpField
{
    ICSharpField ICSharpField.ProtectedReadOnly() => wrapped.ProtectedReadOnly();

    ICSharpField ICSharpField.Protected() => wrapped.Protected();

    ICSharpField ICSharpField.Protected(string value) => wrapped.Protected(value);

    ICSharpField ICSharpField.PrivateReadOnly() => wrapped.PrivateReadOnly();

    ICSharpField ICSharpField.Private() => wrapped.Private();

    ICSharpField ICSharpField.Private(string value) => wrapped.Private(value);

    ICSharpField ICSharpField.Constant(string value) => wrapped.Constant(value);

    ICSharpField ICSharpField.PrivateConstant(string value) => wrapped.PrivateConstant(value);

    ICSharpField ICSharpField.Static() => wrapped.Static();

    ICSharpField ICSharpField.Required() => wrapped.Required();

    ICSharpField ICSharpField.CanBeNull() => wrapped.CanBeNull();

    ICSharpField ICSharpField.WithAssignment(ICSharpStatement value) => wrapped.WithAssignment((CSharpStatement)value);

    ICSharpField ICSharpField.WithInstantiation(ITypeReference model) => wrapped.WithInstantiation(model);
}