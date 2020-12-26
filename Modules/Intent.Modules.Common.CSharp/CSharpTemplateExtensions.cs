using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.Templates
{
    public static class CSharpTemplateExtensions
    {
        public static string ResolveAllUsings(this ITemplate template, ISoftwareFactoryExecutionContext context, params string[] namespacesToIgnore)
        {
            var usings = template
                .GetAllTemplateDependencies()
                .SelectMany(context.FindTemplateInstances<ITemplate>)
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
    }
}