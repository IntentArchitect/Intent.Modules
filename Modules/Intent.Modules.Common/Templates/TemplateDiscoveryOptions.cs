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
        /// Whether or not to automatically track the template as a dependency. Defaults to <see langword="true"/>.
        /// </summary>
        public bool TrackDependency { get; set; } = true;

        /// <summary>
        /// An instance of <see cref="TemplateDiscoveryOptions"/> where <see cref="ThrowIfNotFound"/> is set to false.
        /// </summary>
        public static TemplateDiscoveryOptions DoNotThrow { get; } = new TemplateDiscoveryOptions { ThrowIfNotFound = false };
    }
}