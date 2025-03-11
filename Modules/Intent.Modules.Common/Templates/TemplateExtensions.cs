using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Humanizer;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public static class TemplateExtensions
    {
        public static IEnumerable<ITemplateDependency> GetAllTemplateDependencies(this ITemplate template)
        {
            return template.GetAll<IHasTemplateDependencies, ITemplateDependency>(i => i.GetTemplateDependencies());
        }

        public static IEnumerable<string> GetAllAdditionalHeaderInformation(this ITemplate template)
        {
            return template.GetAll<IHasAdditionalHeaderInformation, string>(i => i.GetAdditionalHeaderInformation());
        }

        public static bool TryGetModel<TModel>(this ITemplate template, out TModel model) where TModel : class
        {
            model = (template as ITemplateWithModel)?.Model as TModel;
            return model != null;
        }

        public static IEnumerable<TResult> GetAll<TInterface, TResult>(this ITemplate template, Func<TInterface, IEnumerable<TResult>> invoke)
        {
            if (template == null)
            {
                return new TResult[] { };
            }
            var result = new List<TResult>();
            if (template is TInterface tInterface)
            {
                result.AddRange(invoke(tInterface));
            }

            if (template is IExposesDecorators<ITemplateDecorator> hasDecorators)
            {
                result.AddRange(
                    hasDecorators.GetDecorators()
                        .OrderByDescending(d => d.Priority) // Higher value = Higher priority
                        .OfType<TInterface>()
                        .SelectMany(invoke)
                        .Distinct()
                        .ToArray()
                    );
            }
            return result.Distinct();
        }

        /// <summary>
        /// Converts the input to sense casing (e.g. SomeString -> Some string).
        /// </summary>
        public static string ToSentenceCase(this string name)
        {
            return name.Humanize(LetterCasing.Sentence);
        }

        //public static string ToCamelCase(this string name) => name?.Replace('-', ' ').Camelize();
        [FixFor_Version4("Use above commented out implementation instead which uses Humanizer's .Camelize() Humaizer does not align with asp.net core")]
        public static string ToCamelCase(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (char.IsLower(name[0]))
            {
                return name;
            }

            return char.ToLower(name[0]) + name[1..];
        }

        //This Algorithm is from System.Text.Json source code : `JsonCamelCaseNamingPolicy`
        public static string ToCamelCase(this string name, bool strict)
        {
            if (!strict)
            {
                return name.ToCamelCase();
            }

            if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
            {
                return name;
            }

            return string.Create(name.Length, name, (chars, name) =>
            {
                //.AsSpan can be removed when we upgrade to .Net8
                name.AsSpan().CopyTo(chars);
                FixCasing(chars);
            });

        }

        private static void FixCasing(Span<char> chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);

                // Stop when next char is already lowercase.
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (chars[i + 1] == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }
        }

        //public static string ToPascalCase(this string name) => name?.Replace('-', ' ').Pascalize();
        [FixFor_Version4("Use above commented out implementation instead which uses Humanizer's .Pascalize()")]
        public static string ToPascalCase(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            var result = new StringBuilder(name.Length);

            for (var i = 0; i < name.Length; i++)
            {
                var @char = name[i];

                if (@char is '-' or ' ')
                {
                    continue;
                }

                if (result.Length == 0 || name[i - 1] is '-' or ' ')
                {
                    result.Append(char.ToUpperInvariant(@char));
                    continue;
                }

                result.Append(@char);
            }

            return result.ToString();
        }

        public static string ToDotCase(this string name) => name?.Replace('-', ' ').Underscore().Replace('_', '.');

        public static string ToKebabCase(this string name) => name?.Kebaberize();

        public static string ToSnakeCase(this string name) => name?.Replace('-', ' ').Underscore();

        /// <summary>
        /// Obsolete. Use <see cref="Pluralize"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static string ToPluralName(this string s) => Pluralize(s);

        public static string RemovePrefix(this string word, params string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (word.StartsWith(prefix))
                {
                    word = word.Substring(prefix.Length);
                }
            }

            return word;
        }

        public static string RemoveSuffix(this string word, params string[] suffixes)
        {
            // Because the suffixes could appear in a different order, we want to run the removals
            // also for the number of suffixes to remove.
            foreach (var _ in suffixes)
            {
                foreach (var suffix in suffixes)
                {
                    if (word.EndsWith(suffix))
                    {
                        word = word.Substring(0, word.Length - suffix.Length);
                    }
                }
            }

            return word;
        }

        public static string EnsureSuffixedWith(this string @string, string suffix, params string[] suffixesToRemove)
        {
            @string = @string.RemoveSuffix(suffixesToRemove);

            return @string.EndsWith(suffix)
                ? @string
                : $"{@string}{suffix}";
        }

        /// <summary>
        /// Pluralizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be pluralized</param>
        /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = false)
        {
            return InflectorExtensions.Pluralize(word, inputIsKnownToBeSingular);
        }

        /// <summary>
        /// Singularizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be singularized</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
        public static string Singularize(this string word, bool inputIsKnownToBePlural = false)
        {
            return InflectorExtensions.Singularize(word, inputIsKnownToBePlural);
        }

        /// <summary>
        /// This is based on (but not entirely the same) as the `ToPropertyName` method in Common.CSharp.
        /// This does a cleanup of the string in a very similar/same manner as the UI does a cleanup of the 
        /// attribute name, removing any special characters etc
        /// </summary>
        /// <param name="value">The string value to sanitize</param>
        /// <returns>The sanitized value</returns>
        public static string ToSanitized(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var asCharArray = value.ToCharArray();
            for (var i = 0; i < asCharArray.Length; i++)
            {
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

            value = new string(asCharArray);

            // Replace double spaces
            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ");
            }

            value = string.Concat(value
                .Split(' ')
                .Where(element => !string.IsNullOrWhiteSpace(element))
                .Select((element, index) => index == 0
                    ? element
                    : element.ToPascalCase()));
            
            return value;
        }
    }

}
