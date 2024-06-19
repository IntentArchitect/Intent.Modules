using System;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpStatement : ICSharpMetadataBase, ICodeBlock, ICSharpExpression
{
    IHasCSharpStatementsActual Parent { get; set; }
    string Text { get; set; }
    ICSharpStatement SeparatedFromPrevious();
    ICSharpStatement SeparatedFromNext();
    ICSharpStatement Indent();
    ICSharpStatement Outdent();
    ICSharpStatement SetIndent(string relativeIndentation);
    ICSharpStatement WithSemicolon();
    ICSharpStatement InsertAbove(ICSharpStatement statement, Action<ICSharpStatement> configure = null);
    ICSharpStatement InsertAbove(string statement, Action<ICSharpStatement> configure = null) => InsertAbove(new CSharpStatement(statement), configure);
    ICSharpStatement InsertAbove(params ICSharpStatement[] statements);
    ICSharpStatement InsertBelow(ICSharpStatement statement, Action<ICSharpStatement> configure = null);
    ICSharpStatement InsertBelow(params ICSharpStatement[] statements);
    ICSharpStatement FindAndReplace(string find, string replaceWith);
    void Replace(string replaceWith) => Replace(new CSharpStatement(replaceWith));
    void Replace(ICSharpStatement replaceWith);
    void Remove();
    new string GetText(string indentation);
}