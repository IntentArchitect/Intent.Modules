namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.HooksJson
{
    /// <summary>
    /// A marker, carrying no data on purpose. Which harness a template instance is generating for
    /// is decided by the AI.Context anchor folder it landed in, read from the output target - not
    /// by anything modelled here. The type remains only because the registration is file-per-model.
    /// </summary>
    public class HarnessFolderModel
    {
    }
}
