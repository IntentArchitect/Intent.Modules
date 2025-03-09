using System;
using Intent.SdkEvolutionHelpers;
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
        /// Key value for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate the element
        /// type to be removed for the file in the MSBuild file.
        /// </summary>
        public static readonly string RemoveItemType = "VisualStudio.MsBuildFileItem.RemovedItemType";

        /// <summary>
        /// Obsolete. Use <see cref="MsBuildFileItemGenerationBehaviour"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static readonly string AlwaysGenerateProjectItem = "VisualStudio.MsBuildFileItem.IsNeverImplicitlyInSdkStyleProjects";

        /// <summary>
        /// Key value for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate the
        /// generation behaviour of its entry in the MSBuild file. Accepts <see langword="string"/>
        /// representations of <see cref="VisualStudio.MsBuildFileItemGenerationBehaviour"/>.
        /// </summary>
        public static readonly string MsBuildFileItemGenerationBehaviour = "VisualStudio.MsBuildFileItem.GenerationBehaviour";

        /// <summary>
        /// Key value for <see cref="ITemplateFileConfig.CustomMetadata"/> to indicate the element
        /// type for the file in the MSBuild file was added implicitly.
        /// </summary>
        public static readonly string WasAddedImplicitly = "VisualStudio.MsBuildFileItem.WasAddedImplicitly";
    }
}