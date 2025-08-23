namespace Intent.Modules.Common.Templates
{
    /// <summary>
    /// Template discovery options.
    /// </summary>
    public class TemplateDiscoveryOptions
    {
        /// <summary>
        /// Throw an exception if the template is not found. Defaults to <see langword="true"/>.
        /// </summary>
        public bool ThrowIfNotFound { get; set; } = true;

        /// <summary>
        /// Whether to automatically track the template as a dependency. Defaults to <see langword="true"/>.
        /// </summary>
        public bool TrackDependency { get; set; } = true;

        /// <summary>
        /// Whether the template instance to find must be accessible through references defined in the
        /// output targeting designer. When <see langword="null"/> will first try to search with a value
        /// of <see langword="true"/> and if no results are found will then try again with a value of
        /// <see langword="false"/>.
        /// </summary>
        public bool? IsAccessible { get; set; }

        /// <summary>
        /// An instance of <see cref="TemplateDiscoveryOptions"/> where <see cref="ThrowIfNotFound"/> is set to false.
        /// </summary>
        public static TemplateDiscoveryOptions DoNotThrow { get; } = new TemplateDiscoveryOptions { ThrowIfNotFound = false };
    }
}