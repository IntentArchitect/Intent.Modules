using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    internal class CSharpProject : ICSharpProject
    {
        private readonly Lazy<bool> _isNullableAwareContext;
        private readonly IOutputTarget _outputTarget;

        public CSharpProject(IOutputTarget outputTarget)
        {
            _outputTarget = outputTarget;
            _isNullableAwareContext = new Lazy<bool>(GetIsNullableAwareContext);

            if (_outputTarget.Metadata.TryGetValue("Language Version", out var languageVersion))
            {
                LanguageVersion = languageVersion as string;
            }
            if (_outputTarget.Metadata.TryGetValue("Nullable Enabled", out var nullableEnabled))
            {
                NullableEnabled = nullableEnabled as bool? ?? false;
            }
            if (_outputTarget.Metadata.TryGetValue("Target Frameworks", out var targetFrameworks))
            {
                TargetFrameworks = targetFrameworks as IEnumerable<string>;
            }
        }

        public string Id => _outputTarget.Id;
        public string Name => _outputTarget.Name;
        public string Location => _outputTarget.Location;
        public string RelativeLocation => _outputTarget.RelativeLocation;
        public string Type => _outputTarget.Type;
        public IOutputTarget Parent => _outputTarget.Parent;
        public IEnumerable<ITemplate> TemplateInstances => _outputTarget.TemplateInstances;
        public IDictionary<string, object> Metadata => _outputTarget.Metadata;
        public IApplication Application => _outputTarget.Application;
        public ISoftwareFactoryExecutionContext ExecutionContext => _outputTarget.ExecutionContext;
        public string LanguageVersion { get; }
        public bool NullableEnabled { get; }
        public IEnumerable<string> TargetFrameworks { get; }
        public bool IsNullableAwareContext() => _isNullableAwareContext.Value;
        public bool IsNetCore2App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp2"));
        public bool IsNetCore3App => GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp3"));
        public bool IsNet4App => GetSupportedFrameworks().Any(x => x.StartsWith("net4"));
        public bool IsNet5App => GetSupportedFrameworks().Any(x => x.StartsWith("net5"));
        public bool IsNet6App => GetSupportedFrameworks().Any(x => x.StartsWith("net5"));

        public Version[] TargetDotNetFrameworks => GetSupportedFrameworks().Select(x =>
        {
            Version.TryParse($"{x.RemovePrefix("netcoreapp", "net")}.0", out var ver);
            return ver;
        }).Where(x => x != null).ToArray();

        public bool Equals(IOutputTarget other)
        {
            return _outputTarget.Equals(other);
        }

        public IList<IOutputTarget> GetTargetPath()
        {
            return _outputTarget.GetTargetPath();
        }

        public bool HasRole(string role)
        {
            return _outputTarget.HasRole(role);
        }

        public bool OutputsTemplate(string templateId)
        {
            return _outputTarget.OutputsTemplate(templateId);
        }

        public IEnumerable<string> GetSupportedFrameworks()
        {
            return _outputTarget.GetSupportedFrameworks();
        }

        public bool HasTemplateInstances(string templateId)
        {
            return _outputTarget.HasTemplateInstances(templateId);
        }

        public bool HasTemplateInstances(string templateId, Func<ITemplate, bool> predicate)
        {
            return _outputTarget.HasTemplateInstances(templateId, predicate);
        }

        private bool GetIsNullableAwareContext()
        {
            if (!NullableEnabled)
            {
                return false;
            }

            switch (LanguageVersion)
            {
                case "latest":
                case "preview":
                    return true;
                case "default":
                    return TargetFrameworks.All(DefaultSupportsNullable);
                default:
                    return decimal.Parse(LanguageVersion, CultureInfo.InvariantCulture) >= 8.0m;
            }
        }

        /// <summary>
        /// Based on data from the below links, returns true if the default language version for
        /// the provided <paramref name="frameworkMoniker"/> is at least 8.0.
        /// <br/>
        /// https://docs.microsoft.com/dotnet/csharp/language-reference/configure-language-version#defaults
        /// <br/>
        /// https://docs.microsoft.com/dotnet/standard/frameworks#supported-target-frameworks
        /// </summary>
        private static bool DefaultSupportsNullable(string frameworkMoniker)
        {
            switch (frameworkMoniker)
            {
                case "net11":
                case "net20":
                case "net35":
                case "net40":
                case "net403":
                case "net45":
                case "net451":
                case "net452":
                case "net46":
                case "net461":
                case "net462":
                case "net47":
                case "net471":
                case "net472":
                case "net48":
                    // Language version 7.3
                    return false;
                case "netcoreapp1.0":
                case "netcoreapp1.1":
                case "netcoreapp2.0":
                case "netcoreapp2.1":
                case "netcoreapp2.2":
                    // Language version 7.3
                    return false;
                case "netcoreapp3.0":
                case "netcoreapp3.1":
                    // Language version 8.0
                    return true;
                case "netstandard1.0":
                case "netstandard1.1":
                case "netstandard1.2":
                case "netstandard1.3":
                case "netstandard1.4":
                case "netstandard1.5":
                case "netstandard1.6":
                case "netstandard2.0":
                    // Language version 7.3
                    return false;
                case "netstandard2.1":
                    // Language version 8.0
                    return true;
            }

            // Match for strings like net5.0, net6.0, etc.
            // net5.0 is version 9.0
            if (frameworkMoniker.StartsWith("net") &&
                frameworkMoniker.Contains(".") &&
                decimal.TryParse(frameworkMoniker.Substring(3), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                return true;
            }

            throw new Exception($"Could not determine default language version for framework moniker '{frameworkMoniker}'");
        }
    }
}