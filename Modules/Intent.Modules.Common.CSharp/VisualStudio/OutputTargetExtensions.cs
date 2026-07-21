using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
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
            var collection = csharpProject.NugetPackageInstalls();
            collection.AddRange(packages.Select(p => new NuGetInstall(p)));
        }

        /// <summary>
        /// Adds a NuGet package dependency to a project, with Installation options.
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
        public static void AddNugetPackageInstalls(this ICSharpProject csharpProject, IEnumerable<NuGetInstall> packages)
        {
            var collection = csharpProject.NugetPackageInstalls();
            collection.AddRange(packages);
        }

        private const string NetSettingsStereotypeId = "a490a23f-5397-40a1-a3cb-6da7e0b467c0";

        /// <summary>
        /// Whether the <paramref name="csharpProject"/> has its <c>SDK</c> set to the provided
        /// <paramref name="sdk"/>.
        /// </summary>
        public static bool HasSdk(this ICSharpProject csharpProject, string sdk)
        {
            var stereotype = csharpProject?.InternalElement.GetStereotype(NetSettingsStereotypeId);
            if (stereotype == null)
            {
                return false;
            }

            return stereotype.TryGetProperty("SDK", out var property) &&
                   property.Value == sdk;
        }

        /// <summary>
        /// Whether the <paramref name="csharpProject"/> has its <c>SDK</c> set to
        /// <c>Microsoft.NET.Sdk.Web</c>.
        /// </summary>
        public static bool HasMicrosoftNetSdkWeb(this ICSharpProject csharpProject) => csharpProject.HasSdk("Microsoft.NET.Sdk.Web");

        /// <summary>
        /// Whether the <paramref name="csharpProject"/> has its <c>SDK</c> set to
        /// <c>Microsoft.NET.Sdk.Worker</c>.
        /// </summary>
        public static bool HasMicrosoftNetSdkWorker(this ICSharpProject csharpProject) => csharpProject.HasSdk("Microsoft.NET.Sdk.Worker");
    }
}
