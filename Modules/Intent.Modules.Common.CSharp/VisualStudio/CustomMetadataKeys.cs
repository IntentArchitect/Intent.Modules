using Intent.Templates;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    /// <summary>
    /// Dictionary keys <see cref="ITemplateFileConfig.CustomMetadata"/>.
    /// </summary>
    /// <remarks>
    /// All are intentionally <see keyword="static"/> as <see keyword="const"/> embeds the
    /// value at compile time meaning that if this changes, then referenced modules which were
    /// compiled with the older value would no longer be in alignment.
    /// </remarks>
    public static class CustomMetadataKeys
    {
        /// <summary>
        /// Prefix for keys for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate they
        /// should be applied as nested elements for the file in the MSBuild file.
        /// </summary>
        public static readonly string ElementPrefix = "VisualStudio.MsBuildFileItem.Element.";

        /// <summary>
        /// Prefix for keys for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate they
        /// should be applied as attributes for the file in the MSBuild file.
        /// </summary>
        public static readonly string AttributePrefix = "VisualStudio.MsBuildFileItem.Attribute.";

        /// <summary>
        /// Key value for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate the element
        /// type for the file in the MSBuild file.
        /// </summary>
        public static readonly string ItemType = "VisualStudio.MsBuildFileItem.ItemType";

        /// <summary>
        /// Key value for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate that an
        /// entry should always be created in the MSBuild file regardless of whether or not it was
        /// implicitly included (for example in an SDK style project).
        /// </summary>
        public static readonly string AlwaysGenerateProjectItem = "VisualStudio.MsBuildFileItem.IsNeverImplicitlyInSdkStyleProjects";
    }
}