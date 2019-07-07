namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal enum NuGetScheme
    {
        /// <summary>
        /// New lean format, required for use by .NET Standard and .NET Core project types.
        /// </summary>
        Lean,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use newer PackageReference NuGet scheme.
        /// </summary>
        VerboseWithPackageReference,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use older packages.config NuGet scheme.
        /// </summary>
        VerboseWithPackagesDotConfig,

        /// <summary>
        /// Unsupported / unknown projected type.
        /// </summary>
        Unsupported
    }
}