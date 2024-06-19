using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpGenericTypeConstraint
{
    string GenericTypeParameter { get; }
    IList<string> Types { get; }
    ICSharpGenericTypeConstraint AddType(string typeName);
}