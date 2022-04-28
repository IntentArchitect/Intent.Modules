namespace Intent.Modules.Common.CSharp.VisualStudio
{
    /// <summary>
    /// Possible values for <see cref="TemplateFileConfigExtensions.WithCopyToOutputDirectory{T}"/> which correlates with the
    /// "Copy to Output Directory" file property in Visual Studio.
    /// </summary>
    public enum CopyToOutputDirectory
    {
        /// <summary>
        /// Adds <c>&lt;CopyToOutputDirectory&gt;Never&lt;/CopyToOutputDirectory&gt;</c>
        /// as per the "Do not copy" option in Visual Studio.
        /// </summary>
        DoNotCopy,

        /// <summary>
        /// Adds <c>&lt;CopyToOutputDirectory&gt;Always&lt;/CopyToOutputDirectory&gt;</c>
        /// as per the "Copy always" option in Visual Studio.
        /// </summary>
        CopyAlways,

        /// <summary>
        /// Adds <c>&lt;CopyToOutputDirectory&gt;PreserveNewest&lt;/CopyToOutputDirectory&gt;</c>
        /// as per the "Copy if newer" option in Visual Studio.
        /// </summary>
        CopyIfNewer
    }
}