using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable

namespace Intent.Modules.ModuleBuilder.AI.Workflow.Templates.Hooks
{
    /// <summary>
    /// The merge rules for hook config files this module writes INTO rather than owns:
    /// ".claude/settings.json", ".codex/hooks.json" and ".cursor/hooks.json".
    /// </summary>
    /// <remarks>
    /// Each of these is a single per-project file that other tools and the developer also register
    /// hooks in - Cursor allows only one project hooks file, and Claude Code's also carries
    /// permissions and environment. Owning them outright meant every Software Factory run deleted
    /// whatever anyone else had added. So the rules are: add our entry when absent, rewrite an entry
    /// we generated ourselves in an earlier version, and never touch anything else.
    /// <para>
    /// Deliberately free of any Intent dependency, so the test project compiles this exact file
    /// rather than a hand-maintained port of it - a port is a seam a change can slip through.
    /// </para>
    /// </remarks>
    internal static class HookConfigMerge
    {
        public enum LoadOutcome
        {
            /// <summary>No file yet - start from an empty object.</summary>
            Missing,

            /// <summary>Parsed into a JSON object, ready to merge into.</summary>
            Loaded,

            /// <summary>
            /// Present but not a JSON object. The caller must return <c>existing</c> unchanged:
            /// throwing would fail the consumer's whole Software Factory run over a file this module
            /// does not own, and overwriting it would destroy what the developer wrote.
            /// </summary>
            Unusable,
        }

        public static LoadOutcome Load(string path, out JsonObject root, out string existing, out string problem)
        {
            root = new JsonObject();
            existing = string.Empty;
            problem = string.Empty;

            if (!File.Exists(path))
            {
                return LoadOutcome.Missing;
            }

            existing = File.ReadAllText(path);
            try
            {
                if (JsonNode.Parse(existing) is JsonObject parsed)
                {
                    root = parsed;
                    return LoadOutcome.Loaded;
                }

                problem = "it parses as JSON but is not an object";
                return LoadOutcome.Unusable;
            }
            catch (JsonException exception)
            {
                problem = $"it is not valid JSON ({exception.Message})";
                return LoadOutcome.Unusable;
            }
        }

        public static JsonObject EnsureObject(JsonObject parent, string key)
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
        /// Claude Code and Codex shape: <c>event -> [ { matcher?, hooks: [ { type, command, ... } ] } ]</c>.
        /// </summary>
        /// <remarks>
        /// Three cases, and the distinction between the last two is the whole point:
        /// <list type="bullet">
        /// <item>A hook's command is EXACTLY ours, current or superseded - rewrite it in place, matcher
        /// included. Without this the merge could only ever add, so a correction to the command would
        /// reach new installs only. Searched across every matcher, because a matcher can change too
        /// (Codex's did) and keying on the old one would leave the stale entry running beside the new.</item>
        /// <item>A hook mentions our gate path and the same subcommand but matches nothing we generated
        /// - the developer has adjusted it. Leave it alone, and do not append a near-duplicate.</item>
        /// <item>Neither - add ours.</item>
        /// </list>
        /// </remarks>
        public static void EnsureGroupedHook(
            JsonObject hooks, string eventName, string? matcher, string gatePath, string subcommand,
            JsonObject hook, string[] superseded)
        {
            var entries = EnsureArray(hooks, eventName);
            var command = hook["command"]!.GetValue<string>();

            foreach (var entry in entries.OfType<JsonObject>())
            {
                if (entry["hooks"] is not JsonArray inner)
                {
                    continue;
                }

                var ours = inner.OfType<JsonObject>().FirstOrDefault(h => IsOurs(h, command, superseded));
                if (ours is not null)
                {
                    SetOrRemove(entry, "matcher", matcher);
                    CopyInto(ours, hook);
                    return;
                }
            }

            if (entries.OfType<JsonObject>().Any(entry =>
                    entry["hooks"] is JsonArray inner &&
                    inner.OfType<JsonObject>().Any(h => IsDevelopersOwn(h, gatePath, subcommand))))
            {
                return;
            }

            var added = new JsonObject();
            if (matcher is not null)
            {
                added["matcher"] = matcher;
            }

            added["hooks"] = new JsonArray(hook.DeepClone());
            entries.Add(added);
        }

        /// <summary>
        /// Cursor shape: <c>event -> [ { type, command, matcher?, failClosed? } ]</c>, the same three
        /// cases as <see cref="EnsureGroupedHook"/>.
        /// </summary>
        public static void EnsureFlatHook(
            JsonObject hooks, string eventName, string gatePath, string subcommand, JsonObject hook, string[] superseded)
        {
            var entries = EnsureArray(hooks, eventName);
            var command = hook["command"]!.GetValue<string>();

            var ours = entries.OfType<JsonObject>().FirstOrDefault(h => IsOurs(h, command, superseded));
            if (ours is not null)
            {
                CopyInto(ours, hook);
                return;
            }

            if (entries.OfType<JsonObject>().Any(h => IsDevelopersOwn(h, gatePath, subcommand)))
            {
                return;
            }

            entries.Add(hook.DeepClone());
        }

        /// <summary>
        /// UnsafeRelaxedJsonEscaping because the default HTML-safe encoder renders characters such as
        /// "&gt;" and "&amp;" as escape sequences. Both parse identically, but a developer opening
        /// their own config file should see the command they would type.
        /// </summary>
        public static string Render(JsonObject root) => root.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        private static JsonArray EnsureArray(JsonObject parent, string key)
        {
            if (parent[key] is JsonArray existing)
            {
                return existing;
            }

            var created = new JsonArray();
            parent[key] = created;
            return created;
        }

        private static bool IsOurs(JsonObject hook, string command, string[] superseded)
        {
            var existing = CommandOf(hook);
            return existing is not null &&
                   (string.Equals(existing, command, StringComparison.Ordinal) || superseded.Contains(existing, StringComparer.Ordinal));
        }

        private static bool IsDevelopersOwn(JsonObject hook, string gatePath, string subcommand)
        {
            var existing = CommandOf(hook);
            return existing is not null &&
                   existing.Contains(gatePath, StringComparison.Ordinal) &&
                   existing.Contains(subcommand, StringComparison.Ordinal);
        }

        private static string? CommandOf(JsonObject hook) =>
            hook["command"] is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;

        /// <summary>
        /// Makes <paramref name="target"/> carry exactly the fields we generate, removing an optional
        /// field (such as "commandWindows" or "failClosed") that an earlier version wrote but the
        /// current one does not. Fields we never generate are left alone.
        /// </summary>
        private static void CopyInto(JsonObject target, JsonObject source)
        {
            foreach (var optional in new[] { "commandWindows", "failClosed", "matcher" })
            {
                if (!source.ContainsKey(optional))
                {
                    target.Remove(optional);
                }
            }

            foreach (var (key, value) in source)
            {
                target[key] = value?.DeepClone();
            }
        }

        private static void SetOrRemove(JsonObject target, string key, string? value)
        {
            if (value is null)
            {
                target.Remove(key);
            }
            else
            {
                target[key] = value;
            }
        }
    }
}
