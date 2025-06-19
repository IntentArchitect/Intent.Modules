#nullable enable
using Intent.Metadata.Models;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpField : ICSharpMember<ICSharpField>
{
    string Name { get; }
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
    ICSharpField WithInstantiation();
}