using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Intent.Configuration;
using Intent.Engine;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Tasks
{
    [IntentManaged(Mode.Merge)]
    public class ReleaseNoteUpdater : IModuleTask
    {
        private readonly ISolutionConfig _solution;
        private readonly IMetadataManager _metadataManager;
        private readonly JsonSerializerOptions _serializerOptions;

        [IntentManaged(Mode.Merge)]
        public ReleaseNoteUpdater(ISolutionConfig solution, IMetadataManager metadataManager)
        {
            _solution = solution;
            _metadataManager = metadataManager;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public string TaskTypeId => "Intent.ModuleBuilder.ReleaseNoteUpdater";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Release note updater";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int Order => 0;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Execute(params string[] args)
        {
            Logging.Log.Info($"Args: {string.Join(",", args)}");

            if (args.Length < 1)
                return Fail("Expected 1 argument, received 0.");

            TaskConfig config;
            try
            {
                config = JsonSerializer.Deserialize<TaskConfig>(args[0], _serializerOptions)
                    ?? throw new InvalidOperationException($"Unable to deserialize: {args[0]}");
            }
            catch (Exception e)
            {
                return Fail($"Failed to deserialize task arguments: {e.Message}");
            }

            try
            {
                var outputPath = ModuleOutputPathHelper.ResolveIModSpecDirectory(
                    _metadataManager, _solution, config.ApplicationId);

                if (outputPath == null)
                    return Fail($"Application not found for ID: {config.ApplicationId}");

                var releaseNotesPath = Path.Combine(outputPath, "release-notes.md");

                if (!File.Exists(releaseNotesPath))
                    return Fail($"release-notes.md not found at: {releaseNotesPath}");

                var imodspecVersionCheck = CheckImodSpecVersion(outputPath, config.Version);
                if (imodspecVersionCheck != null)
                    return Fail(imodspecVersionCheck);

                var content = File.ReadAllText(releaseNotesPath);
                var updated = UpdateReleaseNotes(content, config.Version, config.ChangePrefix, config.Description);
                File.WriteAllText(releaseNotesPath, updated);

                Logging.Log.Info($"Updated release-notes.md — version {config.Version}: {config.ChangePrefix}: {config.Description}");
                return "success";
            }
            catch (Exception e)
            {
                Logging.Log.Failure(e);
                return Fail(e.GetBaseException().Message);
            }
        }

        private static string UpdateReleaseNotes(string content, string version, string changePrefix, string description)
        {
            var sectionHeader = $"### Version {version}";
            var newEntry = $"- {changePrefix}: {description}";
            var lineEnding = content.Contains("\r\n") ? "\r\n" : "\n";

            var lines = content.Split('\n')
                .Select(l => l.TrimEnd('\r'))
                .ToList();

            int headerIdx = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].TrimEnd() == sectionHeader)
                {
                    headerIdx = i;
                    break;
                }
            }

            if (headerIdx >= 0)
            {
                // Section already exists — find the last bullet in it and insert after
                int lastBulletIdx = headerIdx;
                for (int i = headerIdx + 1; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("### "))
                        break;
                    if (lines[i].StartsWith("- ") || lines[i].StartsWith("* "))
                        lastBulletIdx = i;
                }
                lines.Insert(lastBulletIdx + 1, newEntry);
            }
            else
            {
                // New section — prepend at the top of the file
                lines.InsertRange(0, new[] { sectionHeader, "", newEntry, "" });
            }

            return string.Join(lineEnding, lines);
        }

        private static string? CheckImodSpecVersion(string outputPath, string requestedVersion)
        {
            var imodspecFile = Directory.GetFiles(outputPath, "*.imodspec").FirstOrDefault();
            if (imodspecFile == null)
                return null;

            var imodspecRaw = XDocument.Load(imodspecFile).Root?.Element("version")?.Value;
            if (string.IsNullOrWhiteSpace(imodspecRaw))
                return null;

            if (!NuGetVersion.TryParse(imodspecRaw, out var imodspecParsed) ||
                !NuGetVersion.TryParse(requestedVersion, out var requestedParsed))
                return null;

            var imodspecBase = new NuGetVersion(imodspecParsed.Major, imodspecParsed.Minor, imodspecParsed.Patch);
            if (imodspecBase > requestedParsed)
            {
                return $"The .imodspec version ({imodspecRaw}) is ahead of the requested version ({requestedVersion}). " +
                       "Please evaluate and fix this version discrepancy before proceeding.";
            }

            return null;
        }

        private string Fail(string reason)
        {
            Logging.Log.Failure(reason);
            return JsonSerializer.Serialize(new { errorMessage = reason });
        }

        private class TaskConfig
        {
            public string ApplicationId { get; set; } = "";
            public string Version { get; set; } = "";
            public string ChangePrefix { get; set; } = "";
            public string Description { get; set; } = "";
        }
    }
}
