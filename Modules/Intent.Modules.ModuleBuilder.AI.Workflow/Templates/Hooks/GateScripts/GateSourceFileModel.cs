namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.GateScripts
{
    /// <summary>
    /// One source file of the file-based gate app emitted into ".agents/hooks/" - either a C#
    /// source file compiled in via "#:include", or the "Directory.Build.props" that shields the
    /// app from the consuming repo's own root Directory.Build.props.
    /// </summary>
    public class GateSourceFileModel
    {
        public GateSourceFileModel(string name, string extension, string content)
        {
            Name = name;
            Extension = extension;
            Content = content;
        }

        /// <summary>File name without extension, e.g. "Cli", "Directory.Build".</summary>
        public string Name { get; }

        /// <summary>File extension without the dot, e.g. "cs", "props".</summary>
        public string Extension { get; }

        /// <summary>The literal file content to emit verbatim.</summary>
        public string Content { get; }
    }
}
