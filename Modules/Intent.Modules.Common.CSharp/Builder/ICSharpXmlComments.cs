using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpXmlComments : ICSharpMetadataBase
{
    IList<string> Statements { get; }
    ICSharpXmlComments AddStatements(string statements);
    ICSharpXmlComments AddStatements(IEnumerable<string> statements);
    bool IsEmpty();
}