using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    public static class OutputTargetExtensions
    {
        public static ICSharpProject GetProject(this IOutputTarget outputTarget)
        {
            return new CSharpProject(outputTarget.GetTargetPath()[0]);
        }

        /// <summary>
        /// Adds a NuGet package dependency to a project.
        /// <para>
        /// Example usage:
        /// <code>
        ///     var project = ExecutionContext.OutputTargets.Single(x => x.HasRole("Role")).GetProject();
        ///     project.AddNugetPackages(nugetPackages);
        /// </code>
        /// </para>
        /// </summary>
        /// <example>
        /// Example usage:
        /// <code>
        ///     var project = ExecutionContext.OutputTargets.Single(x => x.HasRole("Role")).GetProject();
        ///     project.AddNugetPackages(nugetPackages);
        /// </code>
        /// </example>
        /// <param name="csharpProject"></param>
        /// <param name="packages"></param>
        public static void AddNugetPackages(this ICSharpProject csharpProject, IEnumerable<INugetPackageInfo> packages)
        {
            var collection = csharpProject.NugetPackages();
            collection.AddRange(packages);
        }
    }
}
