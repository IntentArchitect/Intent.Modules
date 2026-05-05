using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpBuilderExtensions
{
    public static IEnumerable<CSharpStatement> ConvertToStatements(this string s)
    {
        var lines = s.TrimEnd().Replace("\r\n", "\n")
            .Split("\n")
            .SkipWhile(string.IsNullOrWhiteSpace)
            .Select(x => x.TrimEnd())
            .ToArray();

        // We want to remove leading whitespace, but maintain indentation. This below algorithm
        // will assume that first non-empty line dictates the "base" indentation.

        var firstNonBlankLine = lines.FirstOrDefault();
        if (firstNonBlankLine == null)
        {
            return lines.Select(x => new CSharpStatement(x));
        }

        var leadingWhitespaceLength = firstNonBlankLine.Length - firstNonBlankLine.TrimStart().Length;
        var baseIndentation = firstNonBlankLine[..leadingWhitespaceLength];

        return lines.Select(line =>
        {
            var withoutBaseIndentation = line.StartsWith(baseIndentation)
                ? line[leadingWhitespaceLength..]
                : line;

            // The constructor automatically trims the parameter, hence us setting it by the property
            var statement = new CSharpStatement(null)
            {
                Text = withoutBaseIndentation
            };
            return statement;
        });
    }

    public static void SeparateAll(this IEnumerable<ICodeBlock> statements)
    {
        foreach (var statement in statements)
        {
            statement.BeforeSeparator = CSharpCodeSeparatorType.EmptyLines;
            statement.AfterSeparator = CSharpCodeSeparatorType.EmptyLines;
        }
    }

    public delegate string CodeTextTransformer(int codeBlockIndex, ICodeBlock codeBlock, string indentation);
    public static string ConcatCode(this IEnumerable<ICodeBlock> codeBlocks, string indentation, CodeTextTransformer codeTextTransformer = null)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            //.OrderBy(x => x is CSharpLocalMethod) // we should not prescribe this, and it is hidden from the user and likely to lead to confusion
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select((s, i) => $"{orderedCodeBlocks.DetermineSeparator(i, s, indentation, string.Empty, codeTextTransformer)}"));
    }

    public static string JoinCode(this IEnumerable<ICodeBlock> codeBlocks, string separator, string indentation)
    {
        // It's conventional to always have local methods at the bottom of a code block
        var orderedCodeBlocks = codeBlocks
            //.OrderBy(x => x is CSharpLocalMethod) // we should not prescribe this, and it is hidden from the user and likely to lead to confusion
            .ToArray();

        return string.Concat(orderedCodeBlocks.Select((s, i) => $"{orderedCodeBlocks.DetermineSeparator(i, s, indentation, separator)}"));
    }

    /// <summary>
    /// This method will format the string as an "Initial/Default" value. Used for setting the initial value of a property, for example
    /// Will perform additional checks and formatting on the string
    /// </summary>
    /// <param name="template"></param>
    /// <param name="value"></param>
    /// <param name="typeReference"></param>
    /// <returns></returns>
    public static string AsFormattedValidTypeValue(this string value, ICSharpFileBuilderTemplate template, ITypeReference typeReference)
    {
        var element = typeReference.Element;

        if(value.StartsWith('@'))
        {
            return value[1..];
        }

        if(typeReference.IsCollection)
        {
            return value;
        }
        
        // if it is an enum, and the value does not contain the enum name, but is a literal
        if (element.IsEnumModel() && !value.Contains('.') && element.AsEnumModel().Literals.Any(l => l.Name.Equals(value, StringComparison.OrdinalIgnoreCase)))
        {
            return $"{template.GetTypeName(typeReference)}.{value}";
        }

        // String: wrap in quotes unless it is already quoted, is null/default, or looks like a method call / field reference
        if (element != null && element.IsStringType()
            && !value.StartsWith('"')
            && !value.StartsWith('_')
            && value != "null"
            && value != "default"
            && !value.Contains('(')
            && !value.Contains('.'))
        {
            return $"\"{value}\"";
        }

        // DateTime / Date / DateTimeOffset: prefix bare identifiers (e.g. "Now") with the C# type name
        if ((element.IsDateTimeType() || element.IsDateType() || element.IsDateTimeOffsetType())
            && value != "null"
            && value != "default"
            && !value.Contains('.')
            && !value.Contains('('))
        {
            return $"{template.GetTypeName(typeReference)}.{value}";
        }

        return value;
    }

    private static string DetermineSeparator(this IEnumerable<ICodeBlock> codeBlocks, int codeBlockIndex, ICodeBlock currentCodeBlock, string indentation, string separator = "", CodeTextTransformer codeTextTransformer = null)
    {
        var codeBlocksList = codeBlocks.ToList();
        var currentBlockIndex = codeBlocksList.IndexOf(currentCodeBlock);
        var prevCodeBlock = currentBlockIndex > 0 ? codeBlocksList[currentBlockIndex - 1] : null;

        if (prevCodeBlock is null && currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.None)
        {
            return $"{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
        }

        if (prevCodeBlock is null)
        {
            return $"{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.EmptyLines ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.EmptyLines)
        {
            return $"{Environment.NewLine}{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        if (prevCodeBlock.AfterSeparator is CSharpCodeSeparatorType.NewLine ||
            currentCodeBlock.BeforeSeparator is CSharpCodeSeparatorType.NewLine)
        {
            return $"{Environment.NewLine}{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer)}{(currentBlockIndex < codeBlocksList.Count - 1 ? separator : string.Empty)}";
        }

        return $"{GetCodeText(codeBlockIndex, currentCodeBlock, indentation, codeTextTransformer).TrimStart()}{(currentBlockIndex < codeBlocksList.Count - 1 ? $"{separator} " : string.Empty)}";
    }

    private static string GetCodeText(int codeBlockIndex, ICodeBlock codeBlock, string indentation, CodeTextTransformer codeTextTransformer)
    {
        return codeTextTransformer == null
            ? codeBlock.GetText(indentation)
            : codeTextTransformer(codeBlockIndex, codeBlock, indentation);
    }
}