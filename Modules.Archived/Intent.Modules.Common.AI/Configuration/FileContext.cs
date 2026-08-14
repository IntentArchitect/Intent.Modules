using Intent.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// Provides context for file operations, including configuration directory, location variables,
    /// and template resolution for codebase files.
    /// </summary>
    public class FileContext
    {
        /// <summary>
        /// Gets the directory used as the base for relative paths (e.g. the config file directory).
        /// </summary>
        public string ConfigDirectory { get; }

        /// <summary>
        /// Gets the variables such as applicationLocation -> "C:\\Dev\\MyApp".
        /// Used to replace tokens like @applicationLocation in paths.
        /// </summary>
        public IDictionary<string, string> LocationVariables { get; }

        /// <summary>
        /// Gets the optional resolver for “Template” input files that belong to another system.
        /// Given a template Id, returns one or more codebase files.
        /// </summary>
        public Func<string, IEnumerable<ICodebaseFile>>? TemplateResolver { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileContext"/> class.
        /// </summary>
        /// <param name="configDirectory">The base directory for relative paths.</param>
        /// <param name="locationVariables">Variables for token replacement in paths.</param>
        /// <param name="templateResolver">Optional resolver for template input files.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configDirectory"/> is null.</exception>
        public FileContext(
            string configDirectory,
            IDictionary<string, string>? locationVariables = null,
            Func<string, IEnumerable<ICodebaseFile>>? templateResolver = null)
        {
            ConfigDirectory = configDirectory ?? throw new ArgumentNullException(nameof(configDirectory));
            LocationVariables = locationVariables ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            TemplateResolver = templateResolver;
        }

        /// <summary>
        /// Resolves a path with @variables and optional config-relative behavior.
        /// If <paramref name="requireExisting"/> is true, throws if the resolved path does not exist (as file or directory).
        /// </summary>
        /// <param name="raw">The raw path string, possibly containing variable tokens.</param>
        /// <param name="requireExisting">Whether to require the resolved path to exist.</param>
        /// <returns>The resolved path as a string.</returns>
        /// <exception cref="IOException">Thrown if <paramref name="requireExisting"/> is true and the resolved path does not exist.</exception>
        public string ResolveLocation(string? raw, bool requireExisting = true)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return raw ?? string.Empty;
            }

            var resolved = raw;

            // 1. Expand variables like @applicationOutput, @applicationLocation, etc.
            foreach (var kvp in LocationVariables)
            {
                var token = "@" + kvp.Key; // e.g. @applicationOutput
                resolved = resolved.Replace(token, kvp.Value, StringComparison.OrdinalIgnoreCase);
            }

            // 2. If it's now an absolute path (e.g. from @applicationOutput), 
            //    DO NOT combine with ConfigDirectory.
            if (Path.IsPathRooted(resolved))
            {
                if (requireExisting && !File.Exists(resolved) && !Directory.Exists(resolved))
                {
                    throw new IOException($"Resolved path does not exist: {resolved}");
                }

                return resolved;
            }

            // 3. Not rooted -> treat as config-relative if we have a config directory
            if (!string.IsNullOrEmpty(ConfigDirectory))
            {
                var combined = Path.Combine(ConfigDirectory, resolved);

                if (requireExisting && !File.Exists(combined) && !Directory.Exists(combined))
                {
                    throw new IOException($"Unable to find '{resolved}' (resolved to '{combined}').");
                }

                return combined;
            }

            // 4. No ConfigDirectory: use the relative path as-is (optionally validating)
            if (requireExisting && !File.Exists(resolved) && !Directory.Exists(resolved))
            {
                throw new IOException($"Resolved path does not exist: {resolved}");
            }

            return resolved;
        }
    }
}
