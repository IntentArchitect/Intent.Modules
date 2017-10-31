using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
{
    public static class TemplateExtensions
    {
        public static string ResolveAllUsings(this ITemplate template, IProject project, params string[] namespacesToIgnore)
        {
            var usings = template
                .GetAllTemplateDependancies()
                    .SelectMany(project.FindTemplateInstances<ITemplate>)
                    .Where(ti => ti != null && ti.GetMetaData().CustomMetaData.ContainsKey("Namespace"))
                    .ToList()
                    .Select(x => $"using {x.GetMetaData().CustomMetaData["Namespace"]};")
                .Union(
                    template.GetAllDeclareUsing()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => $"{x}")
                )
                .Except(namespacesToIgnore.Select(x => $"using {x};"))
                .Distinct();

            return usings.Any()
                    ? usings.Aggregate((x, y) => x + Environment.NewLine + y)
                    : string.Empty;
        }

        public static IEnumerable<string> GetAllDeclareUsing(this ITemplate template)
        {
            return template.GetAll<IDeclareUsings, string>((i) => i.DeclareUsings());
        }

        public static IEnumerable<ITemplateDependancy> GetAllTemplateDependancies(this ITemplate template)
        {
            return template.GetAll<IHasTemplateDependencies, ITemplateDependancy>((i) => i.GetTemplateDependencies());
        }

        public static IEnumerable<string> GetAllAdditionalHeaderInformation(this ITemplate template)
        {
            return template.GetAll<IHasAdditionalHeaderInformation, string>((i) => i.GetAdditionalHeaderInformation());
        }

        public static IEnumerable<INugetPackageInfo> GetAllNugetDependencies(this ITemplate template)
        {
            return template.GetAll<IHasNugetDependencies, INugetPackageInfo>((i) => i.GetNugetDependencies());
        }

        /*
        public static IEnumerable<IBowerPackageInfo> GetAllBowerDependencies(this ITemplate template)
        {
            return template.GetAll<IHasBowerDependencies, IBowerPackageInfo>((i) => i.GetBowerDependencies());
        }*/

        public static IEnumerable<IAssemblyReference> GetAllAssemblyDependencies(this ITemplate template)
        {
            return template.GetAll<IHasAssemblyDependencies, IAssemblyReference>((i) => i.GetAssemblyDependencies());
        }

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
            var hasDecorators = template as IHasDecorators<ITemplateDecorator>;
            if (hasDecorators != null)
            {
                result.AddRange(
                    hasDecorators.GetDecorators()
                        .Where(x => interfaceType.IsInstanceOfType(x))
                        .Cast<TInterface>()
                        .SelectMany(x => invoke((TInterface)x))
                        .Distinct()
                        .OrderByDescending(d => (d as IPriorityDecorator) != null ? ((IPriorityDecorator)d).Priority : 0) // Higher value = Higher priority
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
            if (Char.IsUpper(s[0]))
            {
                return s;
            }
            else
            {
                return Char.ToUpper(s[0]) + s.Substring(1);
            }

        }

        public static string ToCamelCase(this string s)
        {
            return s.ToCamelCase(true);
        }

        public static string ToCamelCase(this string s, bool reservedWordEscape)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            string result;
            if (Char.IsLower(s[0]))
            {
                result = s;
            }
            else
            {
                result = Char.ToLower(s[0]) + s.Substring(1);
            }

            if (reservedWordEscape)
            {
                switch (result)
                {
                    case "class":
                    case "namespace":
                        return "@" + result;
                }
            }
            return result;
        }

        public static string ToPrivateMember(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            return "_" + ToCamelCase(s, false);
        }
    }

}
