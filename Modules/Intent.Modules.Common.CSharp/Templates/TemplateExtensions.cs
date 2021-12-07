using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    public static class TemplateExtensions
    {
        public static IEnumerable<INugetPackageInfo> GetAllNugetDependencies(this ITemplate template)
        {
            return template.GetAll<IHasNugetDependencies, INugetPackageInfo>((i) => i.GetNugetDependencies());
        }

        public static IEnumerable<IAssemblyReference> GetAllAssemblyDependencies(this ITemplate template)
        {
            return template.GetAll<IHasAssemblyDependencies, IAssemblyReference>((i) => i.GetAssemblyDependencies());
        }

        public static string ToCamelCase(this string s, bool reservedWordEscape)
        {
            var result = s.ToCamelCase();

            if (reservedWordEscape && Common.Templates.CSharp.ReservedWords.Contains(result))
            {
                return $"@{result}";
            }
            return result;
        }

        public static string AsClassName(this string s)
        {
            if (s.StartsWith("I") && s.Length >= 2 && char.IsUpper(s[1]))
            {
                s = s.Substring(1);
            }
            return s.Replace(".", "");
        }

        /// <summary>
        /// Camel-cases input parameter <paramref name="s"/> and prefixes with an underscore.
        /// </summary>
        public static string ToPrivateMember(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            return "_" + ToCamelCase(s, false);
        }

        /// <summary>
        /// Converts <paramref name="string"/> to a valid C# namespace (e.g. removes disallowed characters).
        /// </summary>
        public static string ToCSharpNamespace(this string @string)
        {
            return string.Join('.', @string.Split('.').Select(ToCSharpIdentifier));
        }

        /// <summary>
        /// Converts <paramref name="string"/> to a valid C# reference type (e.g. removes disallowed characters).
        /// </summary>
        public static string ToCSharpIdentifier(this string @string)
        {
            if (string.IsNullOrWhiteSpace(@string))
            {
                return string.Empty;
            }

            bool IsLetter(char @char)
            {
                switch (char.GetUnicodeCategory(@char))
                {
                    case UnicodeCategory.LetterNumber:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.TitlecaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                        return true;
                    case UnicodeCategory.ClosePunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.Control:
                    case UnicodeCategory.CurrencySymbol:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.EnclosingMark:
                    case UnicodeCategory.FinalQuotePunctuation:
                    case UnicodeCategory.Format:
                    case UnicodeCategory.InitialQuotePunctuation:
                    case UnicodeCategory.LineSeparator:
                    case UnicodeCategory.MathSymbol:
                    case UnicodeCategory.ModifierSymbol:
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.OpenPunctuation:
                    case UnicodeCategory.OtherNotAssigned:
                    case UnicodeCategory.OtherNumber:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.OtherSymbol:
                    case UnicodeCategory.ParagraphSeparator:
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.Surrogate:
                        return false;
                    default:
                        return false;
                };
            }

            // https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names
            // - Identifiers must start with a letter, or _.
            // - Identifiers may contain Unicode letter characters, decimal digit characters,
            //   Unicode connecting characters, Unicode combining characters, or Unicode formatting
            //   characters. For more information on Unicode categories, see the Unicode Category
            //   Database. You can declare identifiers that match C# keywords by using the @ prefix
            //   on the identifier. The @ is not part of the identifier name. For example, @if
            //   declares an identifier named if. These verbatim identifiers are primarily for
            //   interoperability with identifiers declared in other languages.

            @string = @string
                .Replace("#", "Sharp")
                .Replace("&", "And");

            if (@string[0] != '_' &&
                @string[0] != '@' &&
                !IsLetter(@string[0]))
            {
                @string = $"_{@string}";
            }

            var asCharArray = @string.ToCharArray();
            for (var i = 0; i < asCharArray.Length; i++)
            {
                // Underscore character is allowed
                if (asCharArray[i] == '_')
                {
                    continue;
                }

                // First character being an @ is allowed
                if (i == 0 && asCharArray[i] == '@')
                {
                    continue;
                }

                switch (char.GetUnicodeCategory(asCharArray[i]))
                {
                    case UnicodeCategory.DecimalDigitNumber:
                    case UnicodeCategory.LetterNumber:
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.ModifierLetter:
                    case UnicodeCategory.OtherLetter:
                    case UnicodeCategory.TitlecaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.Format:
                        break;
                    case UnicodeCategory.ClosePunctuation:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.Control:
                    case UnicodeCategory.CurrencySymbol:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.EnclosingMark:
                    case UnicodeCategory.FinalQuotePunctuation:
                    case UnicodeCategory.InitialQuotePunctuation:
                    case UnicodeCategory.LineSeparator:
                    case UnicodeCategory.MathSymbol:
                    case UnicodeCategory.ModifierSymbol:
                    case UnicodeCategory.NonSpacingMark:
                    case UnicodeCategory.OpenPunctuation:
                    case UnicodeCategory.OtherNotAssigned:
                    case UnicodeCategory.OtherNumber:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.OtherSymbol:
                    case UnicodeCategory.ParagraphSeparator:
                    case UnicodeCategory.PrivateUse:
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.SpacingCombiningMark:
                    case UnicodeCategory.Surrogate:
                        asCharArray[i] = ' ';
                        break;
                    default:
                        asCharArray[i] = ' ';
                        break;
                }
            }

            @string = new string(asCharArray);

            // Replace double spaces
            while (@string.Contains("  "))
            {
                @string = @string.Replace("  ", " ");
            }

            // Replace double underscores
            while (@string.Contains("__"))
            {
                @string = @string.Replace("__", "_");
            }

            return string.Concat(@string.Split(' ').Select(x => x.ToPascalCase()));
        }
    }

}
