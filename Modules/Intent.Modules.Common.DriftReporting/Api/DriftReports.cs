using System.IO;
using Intent.Engine;

namespace Intent.Modules.Common.DriftReporting.Api
{
    public static class DriftReports
    {
        /// <summary>
        /// The application's drift report as guidance names it, starting with the name of the folder holding the solution's
        /// <c>.isln</c>, e.g. <c>intent/.cache/drift/eShop/DriftReport.mdx</c>: relative to that folder's parent, which is the
        /// repository root when the solution sits in an <c>intent</c> folder there. Not absolute: guidance is committed, and an
        /// absolute path differs per machine.
        /// </summary>
        public static string RelativePath(ISoftwareFactoryExecutionContext context) =>
            RelativePath(context, context.GetApplicationConfig().Name);

        /// <summary>
        /// <see cref="RelativePath(ISoftwareFactoryExecutionContext)"/> with a placeholder for the application's name, e.g.
        /// <c>intent/.cache/drift/&lt;Application Name&gt;/DriftReport.mdx</c>, for guidance every application of the solution
        /// writes to the same file: naming one application there would rewrite the file on every other application's run.
        /// </summary>
        internal static string GuidancePath(ISoftwareFactoryExecutionContext context) =>
            RelativePath(context, "<Application Name>");

        private static string RelativePath(ISoftwareFactoryExecutionContext context, string applicationName)
        {
            var solutionFolder = Path.GetFileName(context.GetSolutionConfig().SolutionRootLocation.TrimEnd('/', '\\'));
            return $"{solutionFolder}/.cache/drift/{applicationName}/DriftReport.mdx";
        }

        /// <summary>
        /// The report's absolute path: <c>.cache/drift/&lt;application&gt;/DriftReport.mdx</c> in the folder holding the
        /// solution's <c>.isln</c>, which every Intent solution's own <c>.gitignore</c> ignores.
        /// </summary>
        internal static string FullPath(ISoftwareFactoryExecutionContext context) =>
            Path.GetFullPath(Path.Combine(context.GetSolutionConfig().SolutionRootLocation, ".cache", "drift", context.GetApplicationConfig().Name, "DriftReport.mdx"));
    }
}
