namespace Intent.Modules.Common.CSharp.VisualStudio;

/// <summary>
/// Possible values for <see cref="CustomMetadataKeys.MsBuildFileItemGenerationBehaviour"/>.
/// </summary>
public enum MsBuildFileItemGenerationBehaviour
{
    /// <summary>
    /// An entry for the file should always be generated in the MSBuild file.
    /// </summary>
    Always,

    /// <summary>
    /// An entry for the file should never be generated in the MSBuild file.
    /// </summary>
    Never
}