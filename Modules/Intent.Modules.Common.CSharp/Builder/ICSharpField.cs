#nullable enable
namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpField : ICSharpMember<ICSharpField>
{
    ICSharpField ProtectedReadOnly();
    ICSharpField Protected();
    ICSharpField Protected(string value);
    ICSharpField PrivateReadOnly();
    ICSharpField Private();
    ICSharpField Private(string value);
    ICSharpField Constant(string value);
    ICSharpField PrivateConstant(string value);
    ICSharpField Static();
    ICSharpField Required();
    ICSharpField CanBeNull();
    ICSharpField WithAssignment(ICSharpStatement value);
    ICSharpField WithAssignment(string value) => WithAssignment(new CSharpStatement(value));
}