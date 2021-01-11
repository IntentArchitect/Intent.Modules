using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /*
        public static IEnumerable<IBowerPackageInfo> GetAllBowerDependencies(this ITemplate template)
        {
            return template.GetAll<IHasBowerDependencies, IBowerPackageInfo>((i) => i.GetBowerDependencies());
        }*/


        public static IEnumerable<TResult> GetAll<TInterface, TResult>(this ITemplate template, Func<TInterface, IEnumerable<TResult>> invoke)
        {
            if (template == null)
            {
                return new TResult[] { };
            }
            var interfaceType = typeof(TInterface);
            var templateType = template.GetType();
            var result = new List<TResult>();
            var supportsInterface = interfaceType.IsAssignableFrom(templateType);
            if (supportsInterface)
            {
                result.AddRange(invoke((TInterface)template));
            }

            if (template is IExposesDecorators<ITemplateDecorator> hasDecorators)
            {
                result.AddRange(
                    hasDecorators.GetDecorators()
                        .Where(x => interfaceType.IsInstanceOfType(x))
                        .OrderByDescending(d => d.Priority) // Higher value = Higher priority
                        .Cast<TInterface>()
                        .SelectMany(invoke)
                        .Distinct()
                        .ToArray()
                    );
            }
            return result.Distinct();
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
            return name.ToSpecialCase("_");
        }

        public static string ToKebabCase(this string name)
        {
            return name.ToSpecialCase("-");
        }

        public static string ToDotCase(this string name)
        {
            return name.ToSpecialCase(".");
        }


        private static string ToSpecialCase(this string name, string separator)
        {
            var sb = new StringBuilder(name);
            for (int i = 0; i < sb.Length; i++)
            {
                var c = sb[i];
                if (char.IsUpper(c))
                {
                    sb.Remove(i, 1);
                    sb.Insert(i, char.ToLower(c));
                    if (i != 0 && i < sb.Length - 1)
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
            }

            return sb.ToString();
        }

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
            foreach (var suffix in suffixes)
            {
                if (word.EndsWith(suffix))
                {
                    word = word.Substring(0, word.Length - suffix.Length);
                }
            }

            return word;
        }

        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);
        }

        public static string Singularize(this string word, bool inputIsKnownToBePlural = true)
        {
            return Vocabularies.Default.Singularize(word, inputIsKnownToBePlural);
        }
    }

}
