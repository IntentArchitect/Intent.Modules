namespace Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;

public interface IMarkdownFileBuilderTemplate : IFileBuilderTemplate
{
    new IMarkdownFile MarkdownFile { get; }
    IFileBuilderBase IFileBuilderTemplate.File => MarkdownFile;

    /// <summary>
    /// <c>true</c> when there is no existing file, or the existing file's embedded hash still matches its
    /// content (the template will regenerate/overwrite it); <c>false</c> when the on-disk file has been
    /// manually modified since last generation (the template will preserve the existing content on write).
    /// </summary>
    bool ContentHashMatchesDisk { get; }
}
