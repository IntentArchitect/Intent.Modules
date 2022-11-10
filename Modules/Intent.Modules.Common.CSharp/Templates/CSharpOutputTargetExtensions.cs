using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// Extension methods for <see cref="IOutputTarget"/> for use with C# templates.
    /// </summary>
    public static class CSharpOutputTargetExtensions
    {
        /// <summary>
        /// Returns a namespace string based on the full path from the <paramref name="target"/>. This will include folders and C# projects.
        /// </summary>
        public static string GetNamespace(this IOutputTarget target)
        {
            return string.Join(".", target.GetTargetPath()
                .Where(x => !x.Metadata.ContainsKey("Namespace Provider") ||
                            x.Metadata["Namespace Provider"] as bool? == true)
                .Select(x => x.Metadata.TryGetValue("Root Namespace", out var value) &&
                             value is string rootNamespace &&
                             !string.IsNullOrWhiteSpace(rootNamespace)
                    ? rootNamespace
                    : x.Name.ToCSharpNamespace()));
        }
    }
}