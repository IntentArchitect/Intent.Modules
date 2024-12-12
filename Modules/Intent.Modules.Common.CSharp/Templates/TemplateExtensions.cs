using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// Extensions methods for authoring C# templates.
    /// </summary>
    public static class TemplateExtensions
    {
        public static IEnumerable<NuGetInstall> GetAllNuGetInstalls(this ITemplate template)
        {
            return template.GetAll<ITemplate, NuGetInstall>((instance) =>
            {
                if (instance is CSharpTemplateBase x)
                {
                    return x.NugetInstalls;
                }

                if (instance is IHasNugetDependencies hasNuGetDependencies)
                {
                    return hasNuGetDependencies.GetNugetDependencies().Select(p => new NuGetInstall(p));
                }

                return [];
            });
        }

        /// <summary>
        /// Aggregates all results from <see cref="IHasNugetDependencies.GetNugetDependencies"/> for a template, including those specified on decorators.
        /// </summary>
        public static IEnumerable<INugetPackageInfo> GetAllNugetDependencies(this ITemplate template)
        {
            return template.GetAll<IHasNugetDependencies, INugetPackageInfo>((i) => i.GetNugetDependencies());
        }

        /// <summary>
        /// Aggregates all results from <see cref="IHasAssemblyDependencies.GetAssemblyDependencies"/> for a template, including those specified on decorators.
        /// </summary>
        public static IEnumerable<IAssemblyReference> GetAllAssemblyDependencies(this ITemplate template)
        {
            return template.GetAll<IHasAssemblyDependencies, IAssemblyReference>((i) => i.GetAssemblyDependencies());
        }

        /// <summary>
        /// Aggregates all results from <see cref="IHasFrameworkDependencies.GetFrameworkDependencies"/> for a template, including those specified on decorators.
        /// </summary>
        public static IEnumerable<string> GetAllFrameworkDependencies(this ITemplate template)
        {
            return template.GetAll<IHasFrameworkDependencies, string>(i => i.GetFrameworkDependencies());
        }

        /// <summary>
        /// Obsolete, use <see cref="ToLocalVariableName"/> or <see cref="ToParameterName"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string ToCamelCase(this string s, bool reservedWordEscape)
        {
            var result = s.ToCamelCase();

            if (reservedWordEscape)
            {
                result = result.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
            }

            return result;
        }

        /// <summary>
        /// Changes the type name from being for an interface to instead be for a class.
        /// </summary>
        /// <remarks>
        /// Any leading 'I' is removed so long as the 2nd character is also uppercase.
        /// </remarks>
        public static string AsClassName(this string s)
        {
            if (s.StartsWith("I") && s.Length >= 2 && char.IsUpper(s[1]))
            {
                s = s[1..];
            }
            return s.Replace(".", "");
        }

        /// <summary>
        /// Obsolete, use <see cref="ToPrivateMemberName"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string ToPrivateMember(this string s)
        {
            return s.ToPrivateMemberName();
        }

        /// <summary>
        /// Applies <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> to
        /// <paramref name="identifier"/> with <see cref="CapitalizationBehaviour.MakeFirstLetterLower"/>
        /// and ensures the result is prefixed with an '_'.
        /// </summary>
        public static string ToPrivateMemberName(this string identifier)
        {
            if (identifier[0] != '_')
            {
                identifier = $"_{identifier}";
            }

            identifier = identifier.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

            return identifier;
        }

        /// <summary>
        /// Applies <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> to
        /// <paramref name="identifier"/> with <see cref="CapitalizationBehaviour.MakeFirstLetterLower"/>.
        /// </summary>
        public static string ToPropertyName(this string identifier)
        {
            return identifier.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper);
        }


        /// <summary>
        /// Applies <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> to
        /// <paramref name="identifier"/> with <see cref="CapitalizationBehaviour.MakeFirstLetterLower"/>.
        /// </summary>
        public static string ToParameterName(this string identifier)
        {
            return identifier.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
        }

        /// <summary>
        /// Applies <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> to
        /// <paramref name="identifier"/> with <see cref="CapitalizationBehaviour.MakeFirstLetterLower"/>.
        /// </summary>
        public static string ToLocalVariableName(this string identifier)
        {
            return identifier.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
        }

        /// <summary>
        /// Applies <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> to
        /// <paramref name="identifier"/> with <see cref="CapitalizationBehaviour.MakeFirstLetterUpper"/>.
        /// </summary>
        public static string ToTypeName(this string identifier)
        {
            return identifier.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper);
        }

        /// <summary>
        /// Converts <paramref name="string"/> to a valid C# namespace (e.g. removes disallowed characters).
        /// </summary>
        public static string ToCSharpNamespace(this string @string)
        {
            return string.Join('.', @string.Split('.').Select(s => s.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)));
        }

        /// <summary>
        /// An overload of <see cref="ToCSharpIdentifier(string,CapitalizationBehaviour)"/> where
        /// <see cref="CapitalizationBehaviour"/> is set to <see cref="CapitalizationBehaviour.MakeFirstLetterUpper"/>.
        /// </summary>
        /// <remarks>
        /// See also: <seealso cref="ToTypeName"/>.
        /// </remarks>
        [FixFor_Version4("Remove this and give the overload's capitalizationBehaviour parameter a default value instead")]
        public static string ToCSharpIdentifier(this string @string)
        {
            return @string.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper);
        }

        /// <summary>
        /// Converts <paramref name="identifier"/> to a valid C#
        /// <see href="https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names">identifier name</see>.
        /// </summary>
        /// <remarks>
        /// The following rules are applied to the <paramref name="identifier"/> in the following order:
        /// <list type="bullet">
        /// <item>If the string is null or whitespace, an empty string is returned.</item>
        /// <item>Occurrences of '#' are replaced with 'Sharp'.</item>
        /// <item>Occurrences of '&amp;' are replaced with 'And'.</item>
        /// <item>Any invalid characters are replaced with a ' ' (these spaces are removed in a subsequent step).</item>
        /// <item>In the event there are multiple words, each except for the first has its first letter capitalized and are then joined together.</item>
        /// <item><paramref name="capitalizationBehaviour"/> is applied to first character.</item>
        /// <item>If the first character is a number, then an '_' prefix is applied.</item>
        /// <item>If the identifier is a <see href="https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/">C# keyword</see>, then an '@' prefix is applied.</item>
        /// </list>
        /// </remarks>
        /// <param name="identifier">The value to change into a C# identifier.</param>
        /// <param name="capitalizationBehaviour">The <see cref="CapitalizationBehaviour"/> to use.</param>
        /// <returns>A valid C# identifier.</returns>
        [FixFor_Version4("Remove the overload and give the capitalizationBehaviour parameter a default value instead")]
        public static string ToCSharpIdentifier(this string identifier, CapitalizationBehaviour capitalizationBehaviour)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                return string.Empty;
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

            identifier = identifier
                .Replace("#", "Sharp")
                .Replace("&", "And");

            var asCharArray = identifier.ToCharArray();
            for (var i = 0; i < asCharArray.Length; i++)
            {
                // Underscore character is allowed
                if (asCharArray[i] == '_')
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

            identifier = new string(asCharArray);

            // Replace double spaces
            while (identifier.Contains("  "))
            {
                identifier = identifier.Replace("  ", " ");
            }

            identifier = string.Concat(identifier
                .Split(' ')
                .Where(element => !string.IsNullOrWhiteSpace(element))
                .Select((element, index) => index == 0
                    ? element
                    : element.ToPascalCase()));

            var leadingUnderscores = string.Empty;
            for (var i = 0; i < identifier.Length; i++)
            {
                if (identifier[i] == '_')
                {
                    continue;
                }

                leadingUnderscores = identifier[..i];
                identifier = identifier[i..];
                break;
            }

            switch (capitalizationBehaviour)
            {
                case CapitalizationBehaviour.AsIs:
                    break;
                case CapitalizationBehaviour.MakeFirstLetterUpper:
                    if (!char.IsUpper(identifier[0]))
                    {
                        identifier = $"{char.ToUpperInvariant(identifier[0])}{identifier[1..]}";
                    }
                    break;
                case CapitalizationBehaviour.MakeFirstLetterLower:
                    if (!char.IsLower(identifier[0]))
                    {
                        identifier = $"{char.ToLowerInvariant(identifier[0])}{identifier[1..]}";
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(capitalizationBehaviour), capitalizationBehaviour, null);
            }

            identifier = $"{leadingUnderscores}{identifier}";

            if (char.IsNumber(identifier[0]))
            {
                identifier = $"_{identifier}";
            }

            if (Common.Templates.CSharp.ReservedWords.Contains(identifier))
            {
                identifier = $"@{identifier}";
            }

            return identifier;
        }
    }
}
