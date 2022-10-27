using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.Java.Templates;

/// <summary>
/// Extension methods for Java.
/// </summary>
public static class JavaIdentifierExtensionMethods
{
    /// <summary>
    /// List of Java keywords and literals as per <see href="https://docs.oracle.com/javase/specs/jls/se17/html/jls-3.html#jls-3.9-110"/>.
    /// </summary>
    public static readonly IReadOnlyCollection<string> KeywordsAndLiterals = new HashSet<string>
    {
        // Reserved keywords (https://docs.oracle.com/javase/specs/jls/se17/html/jls-3.html#jls-ReservedKeyword):
        "abstract",
        "continue",
        "for",
        "new",
        "switch",
        "assert",
        "default",
        "if",
        "package",
        "synchronized",
        "boolean",
        "do",
        "goto",
        "private",
        "this",
        "break",
        "double",
        "implements",
        "protected",
        "throw",
        "byte",
        "else",
        "import",
        "public",
        "throws",
        "case",
        "enum",
        "instanceof",
        "return",
        "transient",
        "catch",
        "extends",
        "int",
        "short",
        "try",
        "char",
        "final",
        "interface",
        "static",
        "void",
        "class",
        "finally",
        "long",
        "strictfp",
        "volatile",
        "const",
        "float",
        "native",
        "super",
        "while",
        "_",
        // Contextual keywords (https://docs.oracle.com/javase/specs/jls/se17/html/jls-3.html#jls-ContextualKeyword):
        "exports",
        "opens",
        "requires",
        "uses",
        "module",
        "permits",
        "sealed",
        "var",
        "non-sealed",
        "provides",
        "to",
        "with",
        "open",
        "record",
        "transitive",
        "yield",
        // Literals:
        "true",
        "false",
        "null"
    };

    /// <summary>
    /// Converts <paramref name="identifier"/> to a valid Java identifier name.
    /// </summary>
    /// <remarks>
    /// The following rules are applied to the <paramref name="identifier"/>:
    /// <list type="bullet">
    /// <item>Any occurrences of <c>-</c> are replaced with <c>_</c>.</item>
    /// <item>If the first character is a valid Java identifier start character, <paramref name="capitalizationBehaviour"/> is applied to it.</item>
    /// <item>If the first character is not a valid Java identifier start character, but is a valid Java identifier part character, the identifier will be prefixed with <c>_</c>.</item>
    /// <item>Any characters which are not valid Java identifier parts are skipped and will cause the next valid Java identifier part character to be capitalized.</item>
    /// <item>If the identifier is a Java keyword, then an '_' prefix is applied.</item>
    /// </list>
    /// </remarks>
    /// <param name="identifier">The value to change as necessary to make it a valid Java identifier.</param>
    /// <param name="capitalizationBehaviour">The <see cref="CapitalizationBehaviour"/> to use.</param>
    /// <returns>A valid Java identifier.</returns>
    public static string ToJavaIdentifier(this string identifier, CapitalizationBehaviour capitalizationBehaviour = CapitalizationBehaviour.MakeFirstLetterLower)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return string.Empty;
        }

        // Give it enough capacity upfront so it should never need to allocate more later
        var stringBuilder = new StringBuilder(identifier.Length + 1);
        var isNewWord = false;
        foreach (var @char in identifier.ToCharArray())
        {
            var charToUse = @char;
            if (charToUse == '-')
            {
                charToUse = '_';
            }

            if (stringBuilder.Length == 0)
            {
                if (IsJavaIdentifierStart(charToUse))
                {
                    stringBuilder.Append(capitalizationBehaviour switch
                    {
                        CapitalizationBehaviour.AsIs => charToUse,
                        CapitalizationBehaviour.MakeFirstLetterLower => char.ToLowerInvariant(charToUse),
                        CapitalizationBehaviour.MakeFirstLetterUpper => char.ToUpperInvariant(charToUse),
                        _ => throw new ArgumentOutOfRangeException(nameof(capitalizationBehaviour), capitalizationBehaviour, null)
                    });
                }
                else if (IsJavaIdentifierPart(charToUse))
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(charToUse);
                }

                continue;
            }

            if (IsJavaIdentifierPart(charToUse))
            {
                if (isNewWord)
                {
                    stringBuilder.Append(char.ToUpperInvariant(charToUse));
                    isNewWord = false;
                }
                else
                {
                    stringBuilder.Append(charToUse);
                }

                continue;
            }

            isNewWord = true;
        }

        var result = stringBuilder.ToString();
        return !KeywordsAndLiterals.Contains(result)
            ? result
            : $"_{result}";
    }

    /// <summary>
    /// Determines if the character (Unicode code point) is permissible as the first character in a
    /// Java identifier.
    /// See <see href="https://developer.android.com/reference/java/lang/Character#isJavaIdentifierStart(int)"/>.
    /// </summary>
    private static bool IsJavaIdentifierStart(char @char)
    {
        var unicodeCategory = char.GetUnicodeCategory(@char);

        return IsLetter(unicodeCategory) || unicodeCategory
            is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter
            or UnicodeCategory.LetterNumber
            or UnicodeCategory.CurrencySymbol
            or UnicodeCategory.ConnectorPunctuation;
    }

    /// <summary>
    /// Determines if the character (Unicode code point) may be part of a Java identifier as other
    /// than the first character.
    /// See <see href="https://developer.android.com/reference/java/lang/Character#isJavaIdentifierPart(int)"/>.
    /// </summary>
    private static bool IsJavaIdentifierPart(char @char)
    {
        var unicodeCategory = char.GetUnicodeCategory(@char);

        return IsLetter(unicodeCategory) || IsIdentifierIgnorable(@char, unicodeCategory) || unicodeCategory
            is UnicodeCategory.CurrencySymbol
            or UnicodeCategory.ConnectorPunctuation
            or UnicodeCategory.DecimalDigitNumber
            or UnicodeCategory.LetterNumber
            or UnicodeCategory.SpacingCombiningMark
            or UnicodeCategory.NonSpacingMark;
    }

    /// <summary>
    /// Determines if the specified character (Unicode code point) should be regarded as an
    /// ignorable character in a Java identifier or a Unicode identifier.
    /// See <see href="https://developer.android.com/reference/java/lang/Character#isIdentifierIgnorable(int)"/>.
    /// </summary>
    private static bool IsIdentifierIgnorable(char @char, UnicodeCategory unicodeCategory)
    {
        return @char
               is >= '\u0000' and <= '\u0008'
               or >= '\u000E' and <= '\u001B'
               or >= '\u007F' and <= '\u009F' ||
               unicodeCategory is UnicodeCategory.Format;
    }

    /// <summary>
    /// Determines if the specified character (Unicode code point) is a letter.
    /// See <see href="https://developer.android.com/reference/java/lang/Character#isLetter(int)"/>.
    /// </summary>
    private static bool IsLetter(UnicodeCategory unicodeCategory)
    {
        return unicodeCategory
            is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter;
    }
}