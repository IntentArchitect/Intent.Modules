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
/// Tool names and argument names match the real server's, because the gate's guard-version hook keys
/// on them: it matches the tool name "run_designer_script" and reads tool_input.applicationId and
/// tool_input.script. A script that sets the module's Version is applied to the .imodspec, as the
/// Software Factory would; any other script is recorded and reported as run, but changes nothing.
/// </remarks>
public static class McpStub
{
    public const string ServerName = "intent-architect";

    /// <summary>The command and arguments a harness launches the stub with.</summary>
    public static (string Command, string[] Args) LaunchCommand(string workspace, string logPath)
    {
        var self = Environment.ProcessPath ?? throw new InvalidOperationException("Cannot locate the runner's own executable.");
        string[] stubArgs = ["mcp-stub", "--workspace", workspace, "--log", logPath];

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
                "tools/call" => Call(request?["params"], workspace, log),
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
            },
            required: ["applicationId", "script"]),
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

    private static JsonObject Call(JsonNode? parameters, string workspace, string? log)
    {
        var name = parameters?["name"]?.GetValue<string>() ?? "";
        var arguments = parameters?["arguments"] as JsonObject ?? new JsonObject();
        var applications = Applications(workspace);

        var wrote = new List<string>();
        var (text, isError) = name switch
        {
            "get_applications" => (JsonSerializer.Serialize(applications.Select(a => new { id = a.Id, name = a.Name, location = a.Folder })), false),
            "run_designer_script" => RunScript(arguments, applications, wrote),
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
                    ["content"] = File.ReadAllText(path),
                })]),
            }.ToJsonString() + Environment.NewLine);
        }

        return new JsonObject
        {
            ["content"] = new JsonArray(new JsonObject { ["type"] = "text", ["text"] = text }),
            ["isError"] = isError,
        };
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
                var elementName = Regex.Match(content, "<name>([^<]+)</name>").Groups[1].Value;
                var elementId = Regex.Match(content, "\\bid=\"([^\"]+)\"").Groups[1].Value;
                if (!new[] { elementName, elementId }.Any(key => key.Length > 0 && (script.Contains($"\"{key}\"") || script.Contains($"'{key}'"))))
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
            : ("Script ran successfully.", false);
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
