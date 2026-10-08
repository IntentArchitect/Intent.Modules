using System;
using System.Text.Json.Nodes;
using Intent.Utils;

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The TransformText body shared by every template that merges into a hook file rather than
    /// owning it: read what is there, merge our entries in, and leave a file we cannot parse alone.
    /// </summary>
    internal static class MergedHookFile
    {
        public static string Transform(string existingPath, string templateId, string harnessName, Action<JsonObject> merge)
        {
            var outcome = HookConfigMerge.Load(existingPath, out var root, out var existing, out var problem);
            if (outcome == HookConfigMerge.LoadOutcome.Unusable)
            {
                // Returning the file's own content leaves it byte-for-byte untouched. Throwing here
                // would fail the consumer's entire Software Factory run over a file this module does
                // not own, and overwriting it would destroy what the developer and other tools wrote.
                Logging.Log.Warning(
                    $"{templateId}: '{existingPath}' was left exactly as it is because {problem}. " +
                    $"The agent gate is not wired into {harnessName} until the file parses as a JSON object.");
                return existing;
            }

            merge(root);
            return HookConfigMerge.Render(root);
        }
    }
}
