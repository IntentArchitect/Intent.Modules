namespace Intent.Modules.NuGet.Installer.HelperTypes
{
    internal enum ProjectType
    {
        /// <summary>
        /// New lean format, required for use by .NET Standard and .NET Core project types.
        /// </summary>
        LeanScheme,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use newer PackageReference NuGet scheme.
        /// </summary>
        VerboseWithPackageReferenceScheme,

        /// <summary>
        /// The old verbose format used by .NET Framework projects, set to use older packages.config NuGet scheme.
        /// </summary>
        VerboseWithPackagesDotConfigScheme,

        /// <summary>
        /// Unsupported / unknown projected type.
        /// </summary>
        Unsupported
    }
}