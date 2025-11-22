using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Builder;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface ICSharpEnum : ICSharpDeclaration<ICSharpEnum>, ICodeBlock, IHasCSharpName
{
    IList<ICSharpEnumLiteral> Literals { get; }
}

public interface ICSharpEnumLiteral : ICSharpDeclaration<ICSharpEnumLiteral>, ICodeBlock
{
    string LiteralName { get; }
    string LiteralValue { get; }
}