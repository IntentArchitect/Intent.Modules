namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.HooksJson
{
    /// <summary>
    /// One root-level AI-harness folder (e.g. ".codex", ".kiro") found to already exist in the
    /// consuming repository, and therefore eligible to receive a hooks config.
    /// </summary>
    public class HarnessFolderModel
    {
        public HarnessFolderModel(string harness, string folderName)
        {
            Harness = harness;
            FolderName = folderName;
        }

        /// <summary>Short harness id used to pick the content/file shape, e.g. "codex", "kiro".</summary>
        public string Harness { get; }

        /// <summary>The root-level folder name this harness owns, e.g. ".codex", ".kiro".</summary>
        public string FolderName { get; }
    }
}
