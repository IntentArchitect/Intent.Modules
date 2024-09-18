using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpCollectionExpression : CSharpStatement, IHasCSharpStatements
{
    public CSharpCollectionExpression(string typeName = null)
    {
        BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
        AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        TypeName = typeName;
    }

    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public bool ItemsOnNewLines { get; set; }
    public bool? Semicolon { get; set; }
    public string TypeName { get; set; }

    public CSharpCollectionExpression AddItem(string statement, Action<CSharpStatement> configure = null)
    {
        var cSharpStatement = new CSharpStatement(statement);
        Statements.Add(cSharpStatement);
        configure?.Invoke(cSharpStatement);
        return this;
    }

    public CSharpCollectionExpression AddItem<T>(T statement, Action<T> configure = null) where T : CSharpStatement
    {
        Statements.Add(statement);
        configure?.Invoke(statement);
        return this;
    }

    public CSharpCollectionExpression WithItemsOnNewLines(bool value = true)
    {
        ItemsOnNewLines = value;
        return this;
    }

    public CSharpCollectionExpression WithSemicolon(bool? value = true)
    {
        Semicolon = value;
        return this;
    }

    public override string GetText(string indentation)
    {
        var semicolon = Semicolon ?? Parent?.IsCodeBlock ?? false ? ";" : string.Empty;
        var typeName = !string.IsNullOrWhiteSpace(TypeName) ? $"({TypeName})" : string.Empty;

        if (!ItemsOnNewLines)
        {
            var statements = Statements
                .Select(x => x.GetText(string.Empty))
                .ToArray();

            var useSingleLine = typeName.Length + statements.Aggregate(0, (x, y) => x + y.Length + ", ".Length) <= 120;
            if (useSingleLine)
            {
                return $"{typeName}[{string.Join(", ", statements)}]{semicolon}";
            }
        }

        if (Statements.Count == 0)
        {
            return $"{typeName}[]{semicolon}";
        }

        var sb = new StringBuilder();
        
        sb.AppendLine($"{indentation}{RelativeIndentation}{typeName}[");

        foreach (var statement in Statements)
        {
            sb.AppendLine($"{statement.GetText(indentation + RelativeIndentation + "    ")},");
        }

        // Remove trailing comma
        sb.Length -= Environment.NewLine.Length + 1;
        sb.AppendLine();

        sb.Append($"{indentation}{RelativeIndentation}]{semicolon}");

        return sb.ToString();
    }

    bool IHasCSharpStatementsActual.IsCodeBlock => false;
}