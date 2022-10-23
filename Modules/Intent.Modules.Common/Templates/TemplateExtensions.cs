using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Humanizer.Inflections;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public static class TemplateExtensions
    {
        public static IEnumerable<ITemplateDependency> GetAllTemplateDependencies(this ITemplate template)
        {
            return template.GetAll<IHasTemplateDependencies, ITemplateDependency>((i) => i.GetTemplateDependencies());
        }

        public static IEnumerable<string> GetAllAdditionalHeaderInformation(this ITemplate template)
        {
            return template.GetAll<IHasAdditionalHeaderInformation, string>((i) => i.GetAdditionalHeaderInformation());
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
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string s)
        {
            return s.Humanize(LetterCasing.Sentence);
        }

        public static string ToPascalCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            if (char.IsUpper(s[0]))
            {
                return s;
            }
            else
            {
                return char.ToUpper(s[0]) + s.Substring(1);
            }
        }

        public static string ToSnakeCase(this string name)
        {
            return name.ToSpecialCase('_');
        }

        public static string ToKebabCase(this string name)
        {
            return name.ToSpecialCase('-');
        }

        public static string ToDotCase(this string name)
        {
            return name.ToSpecialCase('.');
        }


        private static string ToSpecialCase(this string name, char separator)
        {
            var specialCharacters = new[] { '_', '.', '-', separator };
            var sb = new StringBuilder(name);
            for (int i = 0; i < sb.Length; i++)
            {
                var c = sb[i];
                if (char.IsUpper(c))
                {
                    sb.Remove(i, 1);
                    sb.Insert(i, char.ToLower(c));
                    if (i != 0 && i < sb.Length - 1 && !specialCharacters.Contains(sb[i - 1])) // special characters, including separator
                    {
                        sb.Insert(i, separator);
                        i++;
                    }

                    while (i < sb.Length - 1 && char.IsUpper(sb[i + 1]))
                    {
                        i++;
                        c = sb[i];
                        sb.Remove(i, 1);
                        sb.Insert(i, char.ToLower(c));
                    }
                }

                if (i > 0 && (!char.IsNumber(sb[i - 1]) && char.IsNumber(c) || char.IsNumber(sb[i - 1]) && !char.IsNumber(c)))
                {
                    sb.Insert(i, separator); 
                    i++;
                }
            }

            // TODO: This probably isn't ideal from a performance perspective, we should fix the above logic so this isn't needed, there is test covering this (testing with "new_app_1" and "_")
            var result = sb.ToString();
            while (result.Contains($"{separator}{separator}"))
            {
                result = result.Replace($"{separator}{separator}", separator.ToString());
            }

            return result;
        }

        [Obsolete("Use Pluralize() extension method")]
        public static string ToPluralName(this string s)
        {
            return s.EndsWith("y") && !s.EndsWith("ay") && !s.EndsWith("ey") && !s.EndsWith("iy") && !s.EndsWith("oy") && !s.EndsWith("uy")
                ? (s.Substring(0, s.Length - 1) + "ies")
                : s.EndsWith("ings") ? s
                : (s.EndsWith("s") || s.EndsWith("x") || s.EndsWith("z") || s.EndsWith("ch") || s.EndsWith("sh")) ? $"{s}es" : $"{s}s";
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            string result;
            if (char.IsLower(s[0]))
            {
                result = s;
            }
            else
            {
                result = char.ToLower(s[0]) + s.Substring(1);
            }
            return result;
        }

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

        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return InflectorExtensions.Pluralize(word, inputIsKnownToBeSingular);
        }

        public static string Singularize(this string word, bool inputIsKnownToBePlural = true)
        {
            return InflectorExtensions.Singularize(word, inputIsKnownToBePlural);
        }
    }

}
