#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpField : ICSharpMember<ICSharpField>
{
    string Name { get; }
    ICSharpField CanBeNull();
    ICSharpField Constant(bool isConstant = true);
    ICSharpField Constant(string value);
    ICSharpField Constant(string value, bool isConstant);
    ICSharpField Internal(string? value = null);
    ICSharpField Private();
    ICSharpField Private(string value);
    ICSharpField PrivateConstant(string value);
    ICSharpField PrivateReadOnly();
    ICSharpField Protected();
    ICSharpField Protected(string value);
    ICSharpField ProtectedInternal(string? value = null);
    ICSharpField ProtectedReadOnly();
    ICSharpField Public(string? value = null);
    ICSharpField ReadOnly(bool readOnly = true);
    ICSharpField ReadOnly(string value, bool readOnly = true);
    ICSharpField Required();
    ICSharpField Static();
    ICSharpField WithAssignment(ICSharpStatement value);
    ICSharpField WithAssignment(string value) => WithAssignment(new CSharpStatement(value));
    ICSharpField WithInstantiation();
}