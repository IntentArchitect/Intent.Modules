using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace AgentEvals;

/// <summary>
/// A stand-in for the Intent Architect MCP server: just enough of it for an agent to take the route
/// the gate points it to, so "took the right route" is a recorded fact rather than the judge's opinion.
/// Served over stdio by the runner's own executable (`mcp-stub`), so it needs no build of its own.
/// </summary>
/// <remarks>
/// Tool names and argument names match the real server's, because the gate's hooks key on them:
/// guard-version matches the tool name "run_designer_script" and reads tool_input.applicationId and
/// tool_input.script; guard-write matches "write_file", "patch_file" and "delete_code_file" and reads
/// tool_input.filePath, content, searchBlock and replaceBlock. The file tools really write, resolving a
/// relative path against the workspace as Intent resolves it against the open solution's folder. A script that sets the module's Version is applied to the .imodspec, as the
/// Software Factory would; any other script is recorded and reported as run, but changes nothing.
/// run_solution_query answers only searchAvailableModules, from the scenario's feed (see <see cref="Feed"/>), so the
/// version gate's feed check can complete. update_application_settings writes a new description into the
/// .imodspec's summary and description, as the Software Factory would.
/// <para>
/// run_software_factory regenerates the one thing the stub models: the .imodspec's &lt;iconUrl&gt; from the
/// application config's root &lt;icon&gt;. It also stages an unrelated change to an instruction file, as real runs
/// did when the module-svg-icon skill was written, so a check can tell "applied only the .imodspec" from "applied
/// everything". With autoApply left on, it writes every change, as the real tool does for a non-destructive run;
/// with it off, the changes wait for apply_staged_file_changes. get_file_diffs returns the real diff lines, so an
/// icon change's diff carries the whole base64 payload, exactly as Intent's does.
/// </para>
/// </remarks>
public static class McpStub
{
    public const string ServerName = "intent-architect";

    /// <summary>The command and arguments a harness launches the stub with.</summary>
    public static (string Command, string[] Args) LaunchCommand(Workspace workspace)
    {
        var self = Environment.ProcessPath ?? throw new InvalidOperationException("Cannot locate the runner's own executable.");
        string[] stubArgs = ["mcp-stub", "--workspace", workspace.Root, "--log", workspace.StubLogPath, "--published", workspace.PublishedVersion];

        // Run as "dotnet <dll>" rather than as an apphost: pass the assembly too.
        return Path.GetFileNameWithoutExtension(self).Equals("dotnet", StringComparison.OrdinalIgnoreCase)
            ? (self, [Assembly.GetEntryAssembly()!.Location, .. stubArgs])
            : (self, stubArgs);
    }

    public static async Task<int> RunAsync(string[] args)
    {
        string? Value(string name) => Array.IndexOf(args, name) is var i and >= 0 && i + 1 < args.Length ? args[i + 1] : null;
        var workspace = Value("--workspace") ?? Directory.GetCurrentDirectory();
        var log = Value("--log");
        var feed = Feed(workspace, Value("--published") ?? "1.0.0");

        var stdin = Console.In;
        var stdout = Console.Out;
        while (await stdin.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            JsonNode? request;
            try
            {
                request = JsonNode.Parse(line);
            }
            catch (JsonException)
            {
                continue;
            }

            var id = request?["id"]?.DeepClone();
            var method = request?["method"]?.GetValue<string>() ?? "";
            JsonNode? result = method switch
            {
                "initialize" => new JsonObject
                {
                    ["protocolVersion"] = request?["params"]?["protocolVersion"]?.GetValue<string>() ?? "2025-06-18",
                    ["capabilities"] = new JsonObject { ["tools"] = new JsonObject() },
                    ["serverInfo"] = new JsonObject { ["name"] = ServerName, ["version"] = "eval-stub" },
                },
                "tools/list" => new JsonObject { ["tools"] = Tools() },
                "tools/call" => Call(request?["params"], workspace, log, feed),
                "ping" => new JsonObject(),
                _ => null,
            };

            if (id is null)
            {
                continue; // a notification
            }

            var response = result is not null
                ? new JsonObject { ["jsonrpc"] = "2.0", ["id"] = id, ["result"] = result }
                : new JsonObject { ["jsonrpc"] = "2.0", ["id"] = id, ["error"] = new JsonObject { ["code"] = -32601, ["message"] = $"Method not found: {method}" } };
            await stdout.WriteLineAsync(response.ToJsonString());
            await stdout.FlushAsync();
        }

        return 0;
    }

    private static JsonArray Tools() =>
    [
        Tool("get_applications", "Lists the Intent Architect applications in the open solution, with their ids.", new JsonObject()),
        Tool("run_designer_script",
            "Runs a JavaScript designer script against an application's designer model - the supported way to change " +
            "Intent Architect metadata. For a module's version: lookup the package, then " +
            "pkg.ensureStereotype(\"Module Settings\").setProperty(\"Version\", \"X\").",
            new JsonObject
            {
                ["applicationId"] = new JsonObject { ["type"] = "string", ["description"] = "The application's id." },
                ["designerId"] = new JsonObject { ["type"] = "string", ["description"] = "Optional designer id or name." },
                ["script"] = new JsonObject { ["type"] = "string", ["description"] = "The designer script to run." },
                ["includedScriptPaths"] = new JsonObject
                {
                    ["type"] = "array",
                    ["items"] = new JsonObject { ["type"] = "string" },
                    ["description"] = "Optional script files to prepend before `script`: absolute, or relative to the .application.config.",
                },
            },
            required: ["applicationId", "script"]),
        Tool("run_solution_query",
            "Run a read-only JavaScript query against the solution and get its `return` value back as JSON. Globals: " +
            "searchAvailableModules(searchString, { includePrerelease?, repositoryUrl? }).",
            new JsonObject
            {
                ["applicationId"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "Omit when the query only touches modules." },
                ["script"] = new JsonObject { ["type"] = "string", ["description"] = "JavaScript to execute. Its `return` value is the answer." },
            },
            required: ["script"]),
        Tool("update_application_settings", "Partially update an application's settings: basic fields and/or module-contributed settings.",
            new JsonObject
            {
                ["applicationId"] = new JsonObject { ["type"] = "string", ["description"] = "Target application Id." },
                ["description"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "New description." },
                ["name"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "New application name." },
            },
            required: ["applicationId"]),
        Tool("run_software_factory",
            "Run the Software Factory for an application after model changes. In Staging mode it applies the changes to disk itself " +
            "unless any is destructive; pass autoApply false to leave them staged for review.",
            new JsonObject
            {
                ["applicationId"] = new JsonObject { ["type"] = "string", ["description"] = "Target application Id." },
                ["autoApply"] = new JsonObject { ["type"] = "boolean", ["description"] = "Optional, default true. Pass false to leave the changes staged." },
            },
            required: ["applicationId"]),
        Tool("get_file_diffs",
            "Show what the Software Factory changed for one or more files, as a unified diff against the pre-run baseline.",
            new JsonObject
            {
                ["filePaths"] = new JsonObject { ["type"] = "array", ["items"] = new JsonObject { ["type"] = "string" }, ["description"] = "Absolute paths, or paths relative to the application's output folder." },
                ["applicationId"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "Restrict the diff to this application." },
            },
            required: ["filePaths"]),
        Tool("apply_staged_file_changes",
            "Apply the given pending Software Factory staged file changes to disk. Applies ONLY the listed files.",
            new JsonObject
            {
                ["applicationId"] = new JsonObject { ["type"] = "string", ["description"] = "Target application Id." },
                ["relativePaths"] = new JsonObject { ["type"] = "array", ["items"] = new JsonObject { ["type"] = "string" }, ["description"] = "The exact relative file paths to apply." },
            },
            required: ["applicationId", "relativePaths"]),
        Tool("write_file", "Write content to a file, creating it if it doesn't exist or overwriting if it does.",
            new JsonObject
            {
                ["filePath"] = new JsonObject { ["type"] = "string", ["description"] = "Absolute path to the file to write." },
                ["content"] = new JsonObject { ["type"] = "string", ["description"] = "The complete file content to write." },
            },
            required: ["filePath", "content"]),
        Tool("patch_file", "Replace a specific block of text in a file. Pass the literal text to find as searchBlock and its replacement as replaceBlock.",
            new JsonObject
            {
                ["filePath"] = new JsonObject { ["type"] = "string", ["description"] = "Absolute path to the file to patch." },
                ["searchBlock"] = new JsonObject { ["type"] = "string", ["description"] = "Literal text to find." },
                ["replaceBlock"] = new JsonObject { ["type"] = "string", ["description"] = "Literal replacement text." },
            },
            required: ["filePath", "searchBlock", "replaceBlock"]),
        Tool("delete_code_file", "Delete a file.",
            new JsonObject
            {
                ["filePath"] = new JsonObject { ["type"] = "string", ["description"] = "Absolute path to the file to delete." },
            },
            required: ["filePath"]),
    ];

    private static JsonObject Tool(string name, string description, JsonObject properties, string[]? required = null)
    {
        var schema = new JsonObject { ["type"] = "object", ["properties"] = properties };
        if (required is not null)
        {
            schema["required"] = new JsonArray([.. required.Select(r => (JsonNode)r)]);
        }

        return new JsonObject { ["name"] = name, ["description"] = description, ["inputSchema"] = schema };
    }

    private static JsonObject Call(JsonNode? parameters, string workspace, string? log, IReadOnlyList<FeedEntry> feed)
    {
        var name = parameters?["name"]?.GetValue<string>() ?? "";
        var arguments = parameters?["arguments"] as JsonObject ?? new JsonObject();
        var applications = Applications(workspace);

        var wrote = new List<string>();
        var (text, isError) = name switch
        {
            "get_applications" => (JsonSerializer.Serialize(applications.Select(a => new { id = a.Id, name = a.Name, location = a.Folder })), false),
            "run_designer_script" => RunScript(arguments, applications, wrote),
            "run_solution_query" => RunQuery(arguments, workspace, feed),
            "update_application_settings" => UpdateSettings(arguments, applications, wrote),
            "run_software_factory" => RunSoftwareFactory(arguments, applications, workspace, wrote),
            "get_file_diffs" => GetFileDiffs(arguments, applications, workspace),
            "apply_staged_file_changes" => ApplyStaged(arguments, applications, wrote),
            "write_file" or "patch_file" or "delete_code_file" => RunFileTool(name, arguments, workspace, wrote),
            _ => ($"Unknown tool: {name}", true),
        };

        if (log is not null)
        {
            File.AppendAllText(log, new JsonObject
            {
                ["timestamp"] = DateTime.UtcNow.ToString("O"),
                ["tool"] = name,
                ["arguments"] = arguments.DeepClone(),
                ["result"] = text,
                ["isError"] = isError,
                // What the stub itself wrote, with the content, so a check can tell the designer route from a hand-edit.
                ["wrote"] = new JsonArray([.. wrote.Select(path => (JsonNode)new JsonObject
                {
                    ["path"] = Path.GetRelativePath(workspace, path).Replace('\\', '/'),
                    ["content"] = File.Exists(path) ? File.ReadAllText(path) : null,
                })]),
            }.ToJsonString() + Environment.NewLine);
        }

        return new JsonObject
        {
            ["content"] = new JsonArray(new JsonObject { ["type"] = "text", ["text"] = text }),
            ["isError"] = isError,
        };
    }

    private sealed record FeedEntry(string Id, string Version, string Repository);

    /// <summary>
    /// What the module feed offers: SampleModule published at the scenario's published version (1.0.0 unless the
    /// scenario says otherwise), and a local build of whatever version is on disk when the session starts - in
    /// flight, so the version gate leaves it alone. Shaped like the real searchAvailableModules result.
    /// </summary>
    private static IReadOnlyList<FeedEntry> Feed(string workspace, string published)
    {
        var feed = new List<FeedEntry> { new("SampleModule", published, "https://intentarchitect.com/") };
        var onDisk = Directory.EnumerateFiles(workspace, "SampleModule.imodspec", SearchOption.AllDirectories).FirstOrDefault() is { } imodspec
            ? Regex.Match(File.ReadAllText(imodspec), "<version>([^<]+)</version>").Groups[1].Value.Trim()
            : "";
        if (onDisk.Length > 0 && onDisk != published)
        {
            feed.Add(new("SampleModule", onDisk, "local"));
        }

        return feed;
    }

    private static (string Text, bool IsError) RunQuery(JsonObject arguments, string workspace, IReadOnlyList<FeedEntry> feed)
    {
        var script = arguments["script"]?.GetValue<string>() ?? "";
        var search = Regex.Match(script, """searchAvailableModules\(\s*["'`]([^"'`]*)["'`]""");
        if (!search.Success)
        {
            return ("This query is not supported here: only searchAvailableModules(searchString, { includePrerelease }) is.", true);
        }

        var term = search.Groups[1].Value;
        var modules = feed
            .Where(m => m.Id.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(m => new
            {
                id = m.Id,
                summary = "A sample module.",
                authors = "Intent Architect",
                version = m.Version,
                isCompatible = true,
                repository = m.Repository == "local" ? Path.Combine(workspace, "Intent.Modules") : m.Repository,
            });
        return (JsonSerializer.Serialize(new { executed = true, result = modules, output = Array.Empty<string>() }), false);
    }

    private static (string Text, bool IsError) RunScript(JsonObject arguments, IReadOnlyList<Application> applications, List<string> wrote)
    {
        var applicationId = arguments["applicationId"]?.GetValue<string>();
        var script = arguments["script"]?.GetValue<string>() ?? "";
        var application = applications.FirstOrDefault(a => string.Equals(a.Id, applicationId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(a.Name, applicationId, StringComparison.OrdinalIgnoreCase));
        if (application is null)
        {
            return ($"No application with id '{applicationId}'. Call get_applications for the ids.", true);
        }

        var version = Regex.Match(script, """setProperty\(\s*["']Version["']\s*,\s*["']([^"']+)["']\s*\)""");
        if (version.Success && Directory.GetFiles(application.Folder, "*.imodspec").FirstOrDefault() is { } imodspec)
        {
            var content = File.ReadAllText(imodspec);
            File.WriteAllText(imodspec, Regex.Replace(content, "<version>[^<]*</version>", $"<version>{version.Groups[1].Value}</version>"));
            wrote.Add(imodspec);
            return ($"Script ran. {application.Name}'s version is now {version.Groups[1].Value}.", false);
        }

        // A bespoke icon: the included script files are prepended to the script, as the designer does, and a
        // data URI in the result becomes the icon of the element the script names - the stereotype's
        // <icon type="UrlImagePath" source="..."/>. The included files are read here, not by the agent.
        var includedPaths = (arguments["includedScriptPaths"] as JsonArray ?? [])
            .Select(p => p?.GetValue<string>() ?? "")
            .Select(p => Path.GetFullPath(Path.IsPathRooted(p) ? p : Path.Combine(application.Folder, p)))
            .ToList();
        if (includedPaths.FirstOrDefault(p => !File.Exists(p)) is { } missing)
        {
            return ($"Included script not found: {missing}", true);
        }

        var dataUri = Regex.Match(string.Join("\n", includedPaths.Select(File.ReadAllText).Append(script)), "data:image/svg\\+xml;base64,[A-Za-z0-9+/=]+");
        if (dataUri.Success && ScriptElement(application, script) is { } iconTarget)
        {
            var content = File.ReadAllText(iconTarget.File);
            var icon = $"<icon type=\"UrlImagePath\" source=\"{dataUri.Value}\" />";
            var updated = Regex.IsMatch(content, "<icon [^>]*/>")
                ? Regex.Replace(content, "<icon [^>]*/>", icon)
                : content.Replace($"<name>{iconTarget.Name}</name>", $"<name>{iconTarget.Name}</name>\n  {icon}");
            File.WriteAllText(iconTarget.File, updated);
            wrote.Add(iconTarget.File);
            return ($"Script ran. Set {iconTarget.Name}'s icon.", false);
        }

        // Anything else: apply each setProperty("name", "value") to the element the script names, as the
        // designer would - a stub that reports success while changing nothing provokes the very workaround
        // the evals look for. Elements are the XML files under Intent.Metadata, matched by the name or id the
        // script quotes; a property matches a child element ignoring case and spaces ("Default Value" is
        // <defaultValue>).
        var updates = new List<string>();
        var properties = Regex.Matches(script, """setProperty\(\s*["']([^"']+)["']\s*,\s*["']([^"']*)["']\s*\)""");
        var metadata = Path.Combine(application.Folder, "Intent.Metadata");
        if (properties.Count > 0 && Directory.Exists(metadata))
        {
            foreach (var file in Directory.EnumerateFiles(metadata, "*.xml", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);
                if (NamedInScript(content, script) is not { } elementName)
                {
                    continue;
                }

                var updated = content;
                foreach (Match property in properties)
                {
                    var wanted = property.Groups[1].Value.Replace(" ", "");
                    var tag = Regex.Matches(updated, "<([A-Za-z][\\w]*)>[^<]*</\\1>")
                        .Select(m => m.Groups[1].Value)
                        .FirstOrDefault(t => t.Equals(wanted, StringComparison.OrdinalIgnoreCase));
                    if (tag is not null && tag != "name")
                    {
                        updated = Regex.Replace(updated, $"<{tag}>[^<]*</{tag}>", $"<{tag}>{property.Groups[2].Value}</{tag}>");
                        updates.Add($"{elementName}: {tag} = {property.Groups[2].Value}");
                    }
                }

                if (updated != content)
                {
                    File.WriteAllText(file, updated);
                    wrote.Add(file);
                }
            }
        }

        // A script that tried to set something and matched nothing says so, as the designer would, rather
        // than reporting a success that leaves the agent guessing why nothing changed.
        return updates.Count > 0 ? ($"Script ran. Updated {string.Join("; ", updates)}.", false)
            : properties.Count > 0 ? ("Script ran, but changed nothing: no element or property matched the script's setProperty call(s).", false)
            : ("Script ran, but changed nothing and returned no value: it made no setProperty(\"name\", \"value\") call on a named element.", false);
    }

    /// <summary>The element's name when the script quotes its name or id, else null.</summary>
    private static string? NamedInScript(string elementXml, string script)
    {
        var name = Regex.Match(elementXml, "<name>([^<]+)</name>").Groups[1].Value;
        var id = Regex.Match(elementXml, "\\bid=\"([^\"]+)\"").Groups[1].Value;
        return new[] { name, id }.Any(key => key.Length > 0 && (script.Contains($"\"{key}\"") || script.Contains($"'{key}'"))) ? name : null;
    }

    /// <summary>The first designer element (an XML file under Intent.Metadata) the script names, or null.</summary>
    private static (string File, string Name)? ScriptElement(Application application, string script)
    {
        var metadata = Path.Combine(application.Folder, "Intent.Metadata");
        return !Directory.Exists(metadata) ? null
            : Directory.EnumerateFiles(metadata, "*.xml", SearchOption.AllDirectories)
                .Select(file => (File: file, Name: NamedInScript(File.ReadAllText(file), script)))
                .Where(match => match.Name is not null)
                .Select(match => ((string File, string Name)?)(match.File, match.Name!))
                .FirstOrDefault();
    }

    /// <summary>
    /// The Application Settings route for a module's summary: the Software Factory writes the description into the
    /// .imodspec's &lt;summary&gt; and &lt;description&gt;, so the stub writes both, as a run after this call would.
    /// </summary>
    private static (string Text, bool IsError) UpdateSettings(JsonObject arguments, IReadOnlyList<Application> applications, List<string> wrote)
    {
        var applicationId = arguments["applicationId"]?.GetValue<string>();
        var application = applications.FirstOrDefault(a => string.Equals(a.Id, applicationId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(a.Name, applicationId, StringComparison.OrdinalIgnoreCase));
        if (application is null)
        {
            return ($"No application with id '{applicationId}'. Call get_applications for the ids.", true);
        }

        var description = arguments["description"]?.GetValue<string>();
        if (description is null || Directory.GetFiles(application.Folder, "*.imodspec").FirstOrDefault() is not { } imodspec)
        {
            return ("Settings updated. Nothing in the module's files changed.", false);
        }

        var content = File.ReadAllText(imodspec);
        var escaped = System.Security.SecurityElement.Escape(description);
        content = Regex.Replace(content, "<summary>[^<]*</summary>", $"<summary>{escaped}</summary>");
        content = Regex.Replace(content, "<description>[^<]*</description>", $"<description>{escaped}</description>");
        File.WriteAllText(imodspec, content);
        wrote.Add(imodspec);
        return ($"Settings updated. {application.Name}'s description is now '{description}', and the Software Factory wrote it to the .imodspec's <summary> and <description>.", false);
    }

    /// <summary>The run's staged changes, by full path: what each file would become once applied.</summary>
    private static readonly Dictionary<string, string> Staged = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>The unrelated change every run stages, as real runs did beside the icon.</summary>
    public const string UnrelatedStagedFile = ".claude/rules/csharp-guidelines.md";

    private const string UnrelatedStagedLine = "<!-- Regenerated by the Software Factory. -->";

    private static (string Text, bool IsError) RunSoftwareFactory(JsonObject arguments, IReadOnlyList<Application> applications, string workspace, List<string> wrote)
    {
        if (FindApplication(arguments, applications) is not { } application)
        {
            return ($"No application with id '{arguments["applicationId"]}'. Call get_applications for the ids.", true);
        }

        var changes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var config = Directory.GetFiles(application.Folder, "*.application.config").FirstOrDefault();
        var imodspec = Directory.GetFiles(application.Folder, "*.imodspec").FirstOrDefault();
        if (config is not null && imodspec is not null
            && Regex.Match(File.ReadAllText(config), "<icon>([^<]*)</icon>") is { Success: true } icon)
        {
            var current = File.ReadAllText(imodspec);
            var regenerated = Regex.Replace(current, "<iconUrl>[^<]*</iconUrl>", $"<iconUrl>{icon.Groups[1].Value}</iconUrl>");
            if (regenerated != current)
            {
                changes[imodspec] = regenerated;
            }
        }

        var unrelated = Path.GetFullPath(Path.Combine(workspace, UnrelatedStagedFile));
        if (File.Exists(unrelated) && File.ReadAllText(unrelated) is var guidelines && !guidelines.Contains(UnrelatedStagedLine))
        {
            changes[unrelated] = guidelines.TrimEnd() + Environment.NewLine + Environment.NewLine + UnrelatedStagedLine + Environment.NewLine;
        }

        if (changes.Count == 0)
        {
            return ("""{"success":true,"feedback":"Mode: Staging — nothing staged. Software Factory completed.","message":{"numberOfChanges":0,"changes":{}}}""", false);
        }

        var summary = new JsonArray([.. changes.Select(c =>
        {
            var (added, removed) = CountChanges(File.ReadAllText(c.Key), c.Value);
            return (JsonNode)$"{Relative(application, c.Key)} +{added} -{removed}";
        })]);
        // A harness may pass the flag as a string; anything but false keeps the tool's default.
        var apply = !string.Equals(arguments["autoApply"]?.ToString(), "false", StringComparison.OrdinalIgnoreCase);
        foreach (var (path, content) in changes)
        {
            if (apply)
            {
                File.WriteAllText(path, content);
                wrote.Add(path);
                Staged.Remove(path);
            }
            else
            {
                Staged[path] = content;
            }
        }

        return (new JsonObject
        {
            ["success"] = true,
            ["feedback"] = apply
                ? $"Mode: Staging — applied {changes.Count} change(s) to disk. Software Factory completed."
                : $"Mode: Staging — the {changes.Count} change(s) are staged, not yet on disk. Call apply_staged_file_changes to write them.",
            ["message"] = new JsonObject
            {
                ["outputBasePath"] = application.Folder.Replace('\\', '/'),
                ["numberOfChanges"] = changes.Count,
                ["changes"] = new JsonObject { ["overwrite"] = summary },
            },
        }.ToJsonString(), false);
    }

    private static (string Text, bool IsError) GetFileDiffs(JsonObject arguments, IReadOnlyList<Application> applications, string workspace)
    {
        var application = FindApplication(arguments, applications);
        var files = new JsonArray();
        foreach (var requested in (arguments["filePaths"] as JsonArray ?? []).Select(p => p?.GetValue<string>() ?? ""))
        {
            var full = Path.GetFullPath(Path.IsPathRooted(requested) ? requested : Path.Combine(application?.Folder ?? workspace, requested));
            var staged = Staged.TryGetValue(full, out var content);
            files.Add(new JsonObject
            {
                ["filePath"] = full.Replace('\\', '/'),
                ["status"] = staged ? "modified" : File.Exists(full) ? "unchanged" : "not_found",
                ["diff"] = staged ? UnifiedDiff(File.ReadAllText(full), content!) : "",
            });
        }

        return (new JsonObject { ["success"] = true, ["message"] = new JsonObject { ["files"] = files } }.ToJsonString(), false);
    }

    private static (string Text, bool IsError) ApplyStaged(JsonObject arguments, IReadOnlyList<Application> applications, List<string> wrote)
    {
        if (FindApplication(arguments, applications) is not { } application)
        {
            return ($"No application with id '{arguments["applicationId"]}'. Call get_applications for the ids.", true);
        }

        var requested = (arguments["relativePaths"] as JsonArray ?? []).Select(p => p?.GetValue<string>() ?? "").ToList();
        var resolved = requested.Select(p => Path.GetFullPath(Path.IsPathRooted(p) ? p : Path.Combine(application.Folder, p))).ToList();
        var unknown = requested.Where((_, i) => !Staged.ContainsKey(resolved[i])).ToList();
        if (requested.Count == 0 || unknown.Count > 0)
        {
            return ($"Nothing staged for: {string.Join(", ", unknown.DefaultIfEmpty("(no paths given)"))}. Staged: {string.Join(", ", Staged.Keys.Select(k => Relative(application, k)).DefaultIfEmpty("nothing"))}.", true);
        }

        foreach (var path in resolved)
        {
            File.WriteAllText(path, Staged[path]);
            Staged.Remove(path);
            wrote.Add(path);
        }

        return ($"Applied {resolved.Count} file(s) to disk.", false);
    }

    private static Application? FindApplication(JsonObject arguments, IReadOnlyList<Application> applications)
    {
        var applicationId = arguments["applicationId"]?.GetValue<string>();
        return applications.FirstOrDefault(a => string.Equals(a.Id, applicationId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(a.Name, applicationId, StringComparison.OrdinalIgnoreCase));
    }

    private static string Relative(Application application, string path) => Path.GetRelativePath(application.Folder, path).Replace('\\', '/');

    /// <summary>The changed region between two versions of a file: lines removed and added, counted.</summary>
    private static (int Added, int Removed) CountChanges(string before, string after)
    {
        var (_, removed, added) = ChangedRegion(before, after);
        return (added.Length, removed.Length);
    }

    /// <summary>One hunk covering the changed region, with a line of context either side: enough for the stub's single-region changes.</summary>
    private static string UnifiedDiff(string before, string after)
    {
        var (start, removed, added) = ChangedRegion(before, after);
        var old = before.Split('\n');
        var context = start > 0 ? new[] { " " + old[start - 1].TrimEnd('\r') } : [];
        return $"@@ -{start + 1},{removed.Length} +{start + 1},{added.Length} @@\n" +
               string.Join("\n", context.Concat(removed.Select(l => "-" + l)).Concat(added.Select(l => "+" + l)));
    }

    private static (int Start, string[] Removed, string[] Added) ChangedRegion(string before, string after)
    {
        var a = before.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        var b = after.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();
        var start = 0;
        while (start < a.Length && start < b.Length && a[start] == b[start])
        {
            start++;
        }

        var endA = a.Length;
        var endB = b.Length;
        while (endA > start && endB > start && a[endA - 1] == b[endB - 1])
        {
            endA--;
            endB--;
        }

        return (start, a[start..endA], b[start..endB]);
    }

    private static (string Text, bool IsError) RunFileTool(string name, JsonObject arguments, string workspace, List<string> wrote)
    {
        var filePath = arguments["filePath"]?.GetValue<string>() ?? arguments["file_path"]?.GetValue<string>() ?? arguments["path"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return ("filePath is required.", true);
        }

        var full = Path.GetFullPath(Path.IsPathRooted(filePath) ? filePath : Path.Combine(workspace, filePath));
        switch (name)
        {
            case "write_file":
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, arguments["content"]?.GetValue<string>() ?? "");
                break;
            case "patch_file":
                if (!File.Exists(full))
                {
                    return ($"File not found: {filePath}", true);
                }

                var content = File.ReadAllText(full);
                var search = arguments["searchBlock"]?.GetValue<string>() ?? "";
                var index = search.Length == 0 ? -1 : content.IndexOf(search, StringComparison.Ordinal);
                if (index < 0)
                {
                    return ("searchBlock was not found in the file.", true);
                }

                File.WriteAllText(full, content[..index] + (arguments["replaceBlock"]?.GetValue<string>() ?? "") + content[(index + search.Length)..]);
                break;
            default:
                if (!File.Exists(full))
                {
                    return ($"File not found: {filePath}", true);
                }

                File.Delete(full);
                break;
        }

        wrote.Add(full);
        return ($"{name} succeeded for {filePath}.", false);
    }

    private sealed record Application(string Id, string Name, string Folder);

    private static IReadOnlyList<Application> Applications(string workspace) =>
        Directory.EnumerateFiles(workspace, "*.application.config", SearchOption.AllDirectories)
            .Select(path =>
            {
                var content = File.ReadAllText(path);
                var id = Regex.Match(content, "<application[^>]*\\bid=\"([^\"]+)\"").Groups[1].Value;
                var name = Regex.Match(content, "<name>([^<]+)</name>").Groups[1].Value;
                return new Application(id, name, Path.GetDirectoryName(path)!);
            })
            .Where(a => a.Id.Length > 0)
            .ToList();
}
