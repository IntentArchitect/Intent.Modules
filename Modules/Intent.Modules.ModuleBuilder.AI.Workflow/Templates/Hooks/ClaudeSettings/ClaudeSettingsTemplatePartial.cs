using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks.ClaudeSettings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ClaudeSettingsTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.Workflow.Hooks.ClaudeSettings";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ClaudeSettingsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        private const string SettingsFileName = "settings.json";
        private const string HarnessFolder = ".claude";
        private static readonly string GatePath = GateCommands.GatePath(HarnessFolder);

        /// <summary>
        /// Claude Code is the one harness whose hook config lives in a file it does not own. Every
        /// other harness gets a file this module writes outright; ".claude/settings.json" also
        /// carries the developer's permissions, environment and unrelated hooks, so it is merged
        /// rather than generated - existing keys are never overwritten, only absent ones added.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && OutputTarget.Name == ".claude";
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(fileName: "settings", fileExtension: "json", relativeLocation: "");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var existingPath = Path.Combine(OutputTarget.Location, SettingsFileName);
            if (!File.Exists(existingPath))
            {
                return Render(new JsonObject());
            }

            var existing = File.ReadAllText(existingPath);
            JsonObject root;
            try
            {
                root = JsonNode.Parse(existing) as JsonObject;
            }
            catch (JsonException exception)
            {
                // Returning the file's own content leaves it byte-for-byte untouched. Throwing here
                // would fail the consumer's entire Software Factory run over a file this module does
                // not own, and overwriting it would destroy settings the developer wrote by hand.
                Logging.Log.Warning(
                    $"{TemplateId}: '{existingPath}' is not valid JSON ({exception.Message}), so it was left " +
                    "exactly as it is. The agent gate is not wired into Claude Code until the file parses - " +
                    "Claude Code itself also reports a broken settings file at startup.");
                return existing;
            }

            if (root is null)
            {
                Logging.Log.Warning(
                    $"{TemplateId}: '{existingPath}' parses as JSON but is not an object, so it was left " +
                    "exactly as it is and no hooks were added.");
                return existing;
            }

            return Render(root);
        }

        private string Render(JsonObject root)
        {
            var hooks = EnsureObject(root, "hooks");

            EnsureHook(hooks, "SessionStart", matcher: null, command: GateCommands.Warm(HarnessFolder));
            EnsureHook(hooks, "PreToolUse", matcher: "Write|Edit", command: GateCommand("guard-write"));
            EnsureHook(hooks, "PreToolUse", matcher: ".*run_designer_script.*", command: GateCommand("guard-version"));
            EnsureHook(hooks, "Stop", matcher: null, command: GateCommand("close-out"));

            // UnsafeRelaxedJsonEscaping only because the default HTML-safe encoder renders the
            // fail-closed wrapper's "&&" as an escape sequence. Both parse identically, but a
            // developer opening their own settings file should see the command they would type.
            return root.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });
        }

        private static string GateCommand(string command) => GateCommands.Guard(HarnessFolder, command);

        private static JsonObject EnsureObject(JsonObject parent, string key)
        {
            if (parent[key] is JsonObject existing)
            {
                return existing;
            }

            var created = new JsonObject();
            parent[key] = created;
            return created;
        }

        /// <summary>
        /// Adds the entry only when an equivalent one is absent. "Equivalent" is judged on the gate
        /// path appearing in the command, not on an exact string match, so a developer who has
        /// adjusted our command - or an earlier install that wrote a different gate location - is
        /// left alone rather than having a near-duplicate appended beside it on every regeneration.
        /// </summary>
        private static void EnsureHook(JsonObject hooks, string eventName, string matcher, string command)
        {
            if (hooks[eventName] is not JsonArray entries)
            {
                entries = new JsonArray();
                hooks[eventName] = entries;
            }

            foreach (var entry in entries.OfType<JsonObject>())
            {
                var entryMatcher = entry["matcher"]?.GetValue<string>();
                if (!string.Equals(entryMatcher, matcher, StringComparison.Ordinal))
                {
                    continue;
                }

                var alreadyWired = entry["hooks"] is JsonArray inner
                                   && inner.OfType<JsonObject>().Any(h =>
                                       h["command"]?.GetValue<string>()?.Contains(GatePath, StringComparison.Ordinal) == true);
                if (alreadyWired)
                {
                    return;
                }
            }

            var added = new JsonObject();
            if (matcher is not null)
            {
                added["matcher"] = matcher;
            }

            added["hooks"] = new JsonArray(new JsonObject
            {
                ["type"] = "command",
                ["command"] = command,
            });

            entries.Add(added);
        }
    }
}
