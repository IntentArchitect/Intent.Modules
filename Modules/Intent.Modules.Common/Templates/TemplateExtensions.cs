using System;
using System.Collections.Generic;
using System.Linq;
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

        public static string ToCamelCase(this string name) => name?.Replace('-', ' ').Camelize();

        public static string ToPascalCase(this string name) => name?.Replace('-', ' ').Pascalize();

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
        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return InflectorExtensions.Pluralize(word, inputIsKnownToBeSingular);
        }

        /// <summary>
        /// Singularizes the provided input considering irregular words
        /// </summary>
        /// <param name="word">Word to be singularized</param>
        /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
        public static string Singularize(this string word, bool inputIsKnownToBePlural = true)
        {
            return InflectorExtensions.Singularize(word, inputIsKnownToBePlural);
        }
    }

}
