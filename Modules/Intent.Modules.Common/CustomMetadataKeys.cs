using Intent.Templates;

namespace Intent.Modules.Common
{
    /// <summary>
    /// Dictionary keys for <see cref="ITemplateFileConfig.CustomMetadata"/>.
    /// </summary>
    /// <remarks>
    /// All are intentionally <see keyword="static"/> as <see keyword="const"/> embeds the
    /// value at compile time meaning that if this changes, then referenced modules which were
    /// compiled with the older value would no longer be in alignment.
    /// </remarks>
    public static class CustomMetadataKeys
    {
        public static readonly string DefaultMergeMode = "DefaultMergeMode";
    }
    
    /// <summary>
    /// Dictionary values for <see cref="ITemplateFileConfig.CustomMetadata"/>.
    /// </summary>
    /// <remarks>
    /// All are intentionally <see keyword="static"/> as <see keyword="const"/> embeds the
    /// value at compile time meaning that if this changes, then referenced modules which were
    /// compiled with the older value would no longer be in alignment.
    /// </remarks>
    public static class CustomMetadataValues
    {
        public static readonly string MergeModeFully = "Fully";
        public static readonly string MergeModeMerge = "Merge";
        public static readonly string MergeModeIgnore = "Ignore";
    }
}