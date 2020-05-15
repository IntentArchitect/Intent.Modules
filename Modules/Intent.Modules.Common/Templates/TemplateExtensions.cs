using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer.Inflections;
using Intent.Engine;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Templates
{
    public static class TemplateExtensions
    {
        public static string ResolveAllUsings(this ITemplate template, IProject project, params string[] namespacesToIgnore)
        {
            var usings = template
                .GetAllTemplateDependencies()
                    .SelectMany(project.FindTemplateInstances<ITemplate>)
                    .Where(ti => ti != null && ti.GetMetadata().CustomMetadata.ContainsKey("Namespace"))
                    .ToList()
                    .Select(x => x.GetMetadata().CustomMetadata["Namespace"])
                .Union(template.GetAllDeclareUsing())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Except(namespacesToIgnore)
                .Distinct()
                .ToArray();

            foreach (var @using in usings.Where(x => x != FixUsing(x)))
            {
                Logging.Log.Warning($"When resolving usings for Template Id [{template.Id}] file [{template.GetMetadata().FileName}], " +
                                    $"a using arrived with with the format [{@using}], but should have been in the format " +
                                    $"[{FixUsing(@using)}]. The module and/or decorator author should update this module.");
            }

            usings = usings
                .Select(x => $"using {FixUsing(x)};")
                .Distinct()
                .ToArray();

            return usings.Any()
                    ? usings.Aggregate((x, y) => x + Environment.NewLine + y)
                    : string.Empty;
        }

        private static string FixUsing(string @using)
        {
            if (@using.StartsWith("using "))
            {
                @using = @using.Substring("using ".Length, @using.Length - "using ".Length);
            }

            if (@using.EndsWith(";"))
            {
                @using = @using.Substring(0, @using.Length - ";".Length);
            }

            return @using;
        }

        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            return template.GetAll<IDeclareUsings, string>((i) => i.DeclareUsings());
        }

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

        public static string ToKebabCase(this string name)
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
                        sb.Insert(i, "-");
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
                : s.EndsWith("s") ? $"{s}es" : $"{s}s";
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
